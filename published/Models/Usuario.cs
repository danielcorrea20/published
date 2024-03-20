namespace ControlDeFinanzas.Models
{
    public class Usuario
    {
        //misma prop que en la tabla de usuarios en BD
        public int Id { get; set; }
        public string Emial { get; set; }
        public string EmailNormalizado { get; set; }
        public string PasswordHash { get; set; }
    }
}
