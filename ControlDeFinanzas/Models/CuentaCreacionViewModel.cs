using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlDeFinanzas.Models
{
    //hereda de cuentas
    public class CuentaCreacionViewModel : Cuenta
    {
        //selectListItem pra crear select de una forma mas sencilla
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
