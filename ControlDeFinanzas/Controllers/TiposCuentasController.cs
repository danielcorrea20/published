using ControlDeFinanzas.Models;
using ControlDeFinanzas.Servicios;
using Dapper;
using Microsoft.AspNetCore.Mvc;


namespace ControlDeFinanzas.Controllers
{
    public class TiposCuentasController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;

        public TiposCuentasController(IRepositorioTiposCuentas repositorioTiposCuentas,
            IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas)
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            return View(tiposCuentas);
        }
        public IActionResult Crear()
        {

            return View();
        }
        //Recivo la data del formulario
        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuenta)
        {
            //Intercepto que el campo no este relleno o sea un valor nulo para que no se guarde en la BD
            if (!ModelState.IsValid)
            {
                return View(tipoCuenta);
            }

            tipoCuenta.UsuarioId = servicioUsuarios.ObtenerUsuarioId();
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioId);
            if (yaExisteTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre), $"El nombre {tipoCuenta.Nombre} ya existe");
                return View(tipoCuenta);
            }
            await repositorioTiposCuentas.Crear(tipoCuenta);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuentaExiste = await repositorioTiposCuentas.ObtenerPorId(tipoCuenta.Id, usuarioId);

            //Me aseguro que el tipo cuenta existe
            if (tipoCuentaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            //En caso de que exista, actualizo el campo tipo cuenta
            await repositorioTiposCuentas.Actualizar(tipoCuenta);
            //Regresamos al index
            return RedirectToAction("Index");
        }
        //Se refiere a la página cargar el registro po el ID
        [HttpGet]
        public async Task<IActionResult> Editar(int id)

        {
            //La variable usuarioId es igual al metodo ObtenerUsuarioId
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            // variable tipo cuenta que necesita el id del tipo cuenta y del usuario
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            //En caso de que tipo cuenta sea nulo, es decir que el usuario no te permiso para cargar el registro
            if (tipoCuenta == null)
            {
                //Lo redirijo a la vista NoEncontrado
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }
        //Creo la acción de borrar
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            //Me aseguro que el usuario tiene permisos
            if (tipoCuenta == null)
            {
                //Lo redirijo a la vista NoEncontrado
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }
        //Con el post realizamos realomente el borrado
        [HttpPost]
        public async Task<IActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerPorId(id, usuarioId);
            var cuentas = await repositorioCuentas.listaCuenta(usuarioId);
           
            //Me aseguro que el usuario tiene permisos
            if (tipoCuenta == null)
            {
                //Lo redirijo a la vista NoEncontrado
                return RedirectToAction("NoEncontrado", "Home");
            }

            //RECORRER TRANSACCIONES PARA VER SI LA CATEGORIA ESTA EN USO
            foreach (var cuenta in cuentas)
            {
                if (tipoCuenta.Id == cuenta.TipoCuentaId)
                {
                    return RedirectToAction("BorrarTipoCuentaUsada", "Home");
                }
            }

            //Si todo OK realizo la acción borrar
            await repositorioTiposCuentas.Borrar(id);
            //Una vez borrado regresamoa a index
            return RedirectToAction("Index");
        }





        //NUeva verificación personalizada usando REMOVE. Verifico que no tiene el mismo id
        [HttpGet]
        public async Task<IActionResult> VerificarExisteCuenta(string nombre, int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var yaExisteTipoCuenta = await repositorioTiposCuentas.Existe(nombre, usuarioId, id);
            if (yaExisteTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }
        //ACCION QUE SE EJECUTARÁ CUANDO MOVAMOS LOS TIPOS CUENTAS
        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposCuentas = await repositorioTiposCuentas.Obtener(usuarioId);
            var idsTiposCuentas = tiposCuentas.Select(x => x.Id);

            var idsTiposCuentasNoPertenecenAlUsuario = ids.Except(idsTiposCuentas).ToList();

            if (idsTiposCuentasNoPertenecenAlUsuario.Count > 0)
            {
                return Forbid();
            }

            var tiposCuentasOrdenados = ids.Select((valor, indice) =>
                 new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();
            await repositorioTiposCuentas.Ordenar(tiposCuentasOrdenados);
            return Ok();
        }

    }
}
