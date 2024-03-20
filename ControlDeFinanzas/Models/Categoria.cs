using System.ComponentModel.DataAnnotations;

namespace ControlDeFinanzas.Models
{
    //creo la clase categoria con las mismas prop que hay en la BD
    public class Categoria

    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50, ErrorMessage = "No puede ser mayor a {1} dígitos")]
        public string Nombre { get; set; }
        [Display(Name ="Tipo de categoría")]
        public TipoOperacion TipoOperacionId { get; set; }
        public int UsuarioId { get; set; }
    }
}
