namespace ControlDeFinanzas.Models
{
    //modelo para mostrar al usuario
    public class ReporteTransaccionesDetalladas
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; }
        public decimal BalanceDepositos => TransaccionesAgrupadas.Sum(x => x.BalanceDepositos);
        public decimal BalanceRetiros => TransaccionesAgrupadas.Sum(x => x.BalanceRetiros);
        public decimal Total => BalanceDepositos - BalanceRetiros;
        //creo una clase interna pq solo la voy a usar aquí
        public class TransaccionesPorFecha
        {
            public DateTime FechaTransaccion { get; set; }
            public IEnumerable<Transaccion> Transacciones { get; set; }
            public decimal BalanceDepositos => Transacciones.Where(x => x.TipoOperacionId == TipoOperacion.Ingreso).Sum(x => x.Monto);
            public decimal BalanceRetiros => Transacciones.Where(x => x.TipoOperacionId == TipoOperacion.Gasto).Sum(x => x.Monto);
        }

    }
}
