namespace ControlDeFinanzas.Models
{
    public class PaginacionRespuesta
    {
        //página en la que se encentra el usuario
        public int Pagina { get; set; } = 1;
        //Máximos elemento que tendrá la pg
        public int RecordsPorPagina { get; set; } = 10;
        //el total de categorías que tene el usuario
        public int CantidadTotalRecords { get; set; }
        //calculamos las páginas si tengo 100 categorías(records) y quiero mostrar 5 categorías entoncces 100/5 serían 20 páginas
        public int CantidadTotalDePaginas => (int)Math.Ceiling((double)CantidadTotalRecords / RecordsPorPagina);
        public string BaseURL { get; set; }
    }
    //Para poder usar la páginación en distintas pantallas uso un listado de elementos genéricos (T) de estaforma
    //habrá listados de categorías, cuentas o transacciones
    public class PaginacionRespuesta<T> : PaginacionRespuesta
    {
        public IEnumerable<T> Elementos { get; set; }
    }


}
