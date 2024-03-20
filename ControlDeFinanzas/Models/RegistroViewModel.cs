using System.ComponentModel.DataAnnotations;

namespace ControlDeFinanzas.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "El campo debe ser un correo eléctronico válido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        //para que el password salga enmascarado
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
