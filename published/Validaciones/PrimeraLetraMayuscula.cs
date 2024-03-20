using System.ComponentModel.DataAnnotations;

namespace ControlDeFinanzas.Validaciones
{
    public class PrimeraLetraMayuscula : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //Coloco que la validación es OK apesar de que no este el campo relleno, pq en esta vadilación no estoy valorando eso, para eso ya esta el required
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;

            }
            //Creo una variable con l valor de la primera letra del string
            var primeraLetra = value.ToString()[0].ToString();

            //verifico que la primera letra debe ser mayúscula

            if (primeraLetra != primeraLetra.ToUpper())
            {
                return new ValidationResult("La primera letra debe ser mayúscula");
            }
            return ValidationResult.Success;

        }
    }
}
