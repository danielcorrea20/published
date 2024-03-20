using ControlDeFinanzas.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ControlDeFinanzas.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }
        //Vadilacion requerida con su mensaje de error. Uso placeholder para la palabra "nombre"
        [Required(ErrorMessage = "El campo {0} es requerido")]
        //Vadilacion longitud con su mensaje de error. Uso placeholder para la palabra "nombre"
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "La longitud del campo {0} debe ser entre {2} y {1} caracteres")]
        //Coloco el txt del nobre del campo"
        [Display(Name = "Nombre del tipo cuenta")]
        //Llamo a una validación propia creada en la carpeta validaciones
        [PrimeraLetraMayuscula]
        //Validacón para que no se repitan el nombre de las cuentas
        [Remote(action: "VerificarExisteCuenta", controller: "TiposCuentas", AdditionalFields = nameof(Id))]
        public String Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }
    }
}
