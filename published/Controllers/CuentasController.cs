using AutoMapper;
using ControlDeFinanzas.Models;
using ControlDeFinanzas.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlDeFinanzas.Controllers
{
    public class CuentasController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IMapper mapper;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioReportes servicioReportes;

        //inyecto automaper para poder usarlo
        public CuentasController(IRepositorioTiposCuentas repositorioTiposCuentas, IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas,
            IMapper mapper, IRepositorioTransacciones repositorioTransacciones, IServicioReportes servicioReportes)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioReportes = servicioReportes;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentaConTipoCuenta = await repositorioCuentas.Buscar(usuarioId);

            var modelo = cuentaConTipoCuenta
                //como puede haber varios con elmismo tipo cuenta los agrupo por el tipo cuenta
                .GroupBy(x => x.TipoCuenta)
                //hago una proyeccion hacia IndiceCuentasViewModel
                .Select(grupo => new IndiceCuentasViewModel
                {
                    //key es tipo cuenta
                    TipoCuenta = grupo.Key,
                    //grupo.AsEnumerable es el IEnumerable de tiposcuentas
                    Cuentas = grupo.AsEnumerable()
                }
                ).ToList();
            return View(modelo);

        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var modelo = new CuentaCreacionViewModel();
            //HAGO UN MAPEO DE TIPOCUENTA A SELECTLISTIDEM
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            //valido que el tipo cuenta que envia elusuario es valido para el usuarioId
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);

            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEnontrado", "Index");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuenta);
            }

            await repositorioCuentas.crear(cuenta);
            return RedirectToAction("Index");

        }
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            //Me ASEGURO DE QUE LA CUENTA EXISTE
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            //armo el modelo que se espera y utilizo automapper para mapear de cuenta a CuentaCreacionViewModel
            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);


            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
            return View(modelo);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaEditar.Id, usuarioId);

            //confirmo que existe
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(cuentaEditar.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuentas.Actualizar(cuentaEditar);
            return RedirectToAction("Index");
        }
        //creo como siempre las acciones get y post en este caso para borrar
        [HttpGet]
        public async Task<IActionResult> borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(cuenta);
        }
        //en post en donde se realiza el borrado
        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);
            var transacciones = await repositorioTransacciones.listaTransaccionesCuenta(usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            foreach (var transaccion in transacciones)
            {
                if (cuenta.Id == transaccion.CuentaId)
                {
                    return RedirectToAction("BorrarTipoCuentaUsada", "Home");
                }
            }


            await repositorioCuentas.Borrar(id);
            return RedirectToAction("Index");

        }
        private async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));

        }

        public async Task<IActionResult> Detalle(int id, int mes, int año)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);

            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }


            ViewBag.Cuenta = cuenta.Nombre;

            var modelo = await servicioReportes.ObtenerReporteTransaccionesDetalladasPorCuenta(usuarioId, id, mes, año, ViewBag);

            return View(modelo);
        }


    }
}
