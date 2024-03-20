namespace ControlDeFinanzas.Models
{
    public class PaginacionViewModel
    {

        public int Pagina { get; set; } = 1;
        //cantidad por defecto de elementos que habrá en un página
        private int recordsPorPagina = 10;
        //cantidad maxima de elemento ue habrá en una página
        private int cantidadMaximaRecordsPorPagina = 50;

        public int RecordsPorPagina
        {
            get
            {
                return recordsPorPagina;
            }
            set
            //operador ternario: sie el valor que me manda el usuario de cantidad de registros por página es mayor que
            //cantidadMaximaRecordPorPagina entonces por defecto muesto la cantidad máxima (50) 
            //sino muestor el valor si value es 70 se pasa de 50 y muestro 50 si values es 10 muestro 10
            {
                recordsPorPagina = (value > cantidadMaximaRecordsPorPagina)
                    ? cantidadMaximaRecordsPorPagina : value;
            }
            //registro a saltar de página en página


        }
        public int RecordsASaltar => recordsPorPagina * (Pagina - 1);
    }
}

