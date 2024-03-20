using ControlDeFinanzas.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ControlDeFinanzas.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> listaTransacciones(int usuarioId);
        Task<IEnumerable<Transaccion>> listaTransaccionesCuenta(int usuarioId);
    
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
        Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año);
        Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo);
        Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo);
    }
    public class repostorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;
        public repostorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>("Transacciones_Insertar",
                new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                }, commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;
        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar",
                new
                {

                    transaccion.Id,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota,
                    montoAnterior,
                    cuentaAnteriorId
                }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(@"select Transacciones1.*, cat.TipoOperacionId
                                                                            from Transacciones1
                                                                            inner join Categorias cat
                                                                            on cat.id = Transacciones1.CategoriaId
                                                                            where Transacciones1.Id = @Id and Transacciones1.UsuarioId = @usuarioId",
                                                                            new { id, usuarioId });
        }


        public async Task<IEnumerable<Transaccion>> listaTransacciones(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"select Transacciones1.*, cat.TipoOperacionId, cu.TipoCuentaId
                                                                            from Transacciones1
                                                                            inner join Categorias cat
                                                                            on cat.id = Transacciones1.CategoriaId
																			inner join cuentas cu
																			on cu.id = Transacciones1.CuentaId

                                                                            where Transacciones1.UsuarioId = @UsuarioId",
                                                                            new { usuarioId });
        }

        public async Task<IEnumerable<Transaccion>> listaTransaccionesCuenta(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"select Transacciones1.*
                                                                            from Transacciones1
                                                                     
                                                                            where Transacciones1.UsuarioId = @UsuarioId",
                                                                            new { usuarioId });
        }



        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Trasacciones_Borrar", new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"select t.Id, t.Monto, t.FechaTransaccion, c.Nombre as Categoría, 
                                                            cu.Nombre as Cuenta, cu.TipoCuentaId
                                                            from Transacciones1 t
                                                            inner join Categorias c
                                                            on t.CategoriaId=c.Id
                                                            inner join Cuentas cu
                                                            on cu.Id=t.CuentaId
                                                            where t.CuentaId=@CuentaId and t.UsuarioId=@UsuarioId
                                                            and FechaTransaccion between @FechaInicio and @FechaFin", modelo);
        }

        public async Task<IEnumerable<Transaccion>> ObtenerPorUsuarioId(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"select t.Id, t.Monto, t.FechaTransaccion as FechaTransaccion,
                                    c.Nombre as Categoria, cu.Nombre
                                    as Cuenta, c.TipoOperacionId 
                                    as TipoOperacionId, Nota
                                    from Transacciones1 t
                                    inner join categorias c
                                    on c.Id = t.CategoriaId
                                    inner join cuentas cu
                                    on cu.Id = t.CuentaId
                                    where t.UsuarioId = @UsuarioId  
                                    and FechaTransaccion between 
                                    @FechaInicio and @FechaFin
                                    order by t.FechaTransaccion desc", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorSemana>> ObtenerPorSemana(ParametroObtenerTransaccionesPorUsuario modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorSemana>(@" select datediff (d, @fechaInicio, FechaTransaccion) / 7 + 1 as semana,
                                                                            sum(Monto) as Monto, cat.TipoOperacionId
                                                                            from Transacciones1
                                                                            inner join Categorias cat
                                                                            on cat.Id=Transacciones1.CategoriaId
                                                                            where Transacciones1.UsuarioId = @UsuarioId and FechaTransaccion between @FechaInicio and @FechaFin
                                                                            group by DATEDIFF(d, @FechaInicio, FechaTransaccion) /7, cat.TipoOperacionId", modelo);
        }

        public async Task<IEnumerable<ResultadoObtenerPorMes>> ObtenerPorMes(int usuarioId, int año)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultadoObtenerPorMes>(@"select MONTH(FechaTransaccion) as Mes,
                                                                        sum(Monto) as Monto, cat.TipoOperacionId
                                                                        from Transacciones1
                                                                        inner join Categorias cat
                                                                        on cat.Id = Transacciones1.CategoriaId
                                                                        where Transacciones1.UsuarioId = @usuarioId and year(FechaTransaccion) = @Año
                                                                        Group by MONTH(FechaTransaccion), cat.TipoOperacionId", new { usuarioId, año });
        }

    }
}
