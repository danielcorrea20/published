using System.ComponentModel.DataAnnotations;

namespace ControlDeFinanzas.Models
{
    public class Cuenta
    {
        public int Id { get; set; }
        //creo las restricciones
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50)]
        [PrimeraLetraMatuscula]
        public string Nombre { get; set; }
        [Display(Name = "Tipo Cuenta")]
        //tipos cuenta será un select
        public int TipoCuentaId { get; set; }
        public decimal Balance { get; set; }
        [StringLength(maximumLength: 1000)]
        public string Descripcion { get; set; }
        public string TipoCuenta { get; set; }
    }
}
