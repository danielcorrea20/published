using ControlDeFinanzas.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ControlDeFinanzas.Servicios
{
    //Creo la interface para poder trabajar sobre ella
    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task crear(Cuenta cuenta);
        Task<IEnumerable<Cuenta>> listaCuenta(int usuarioId);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioCuentas : IRepositorioCuentas
    {
        private readonly string connectionString;
        //realicion las inyecciones para el conecction string
        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"insert into Cuentas (Nombre, TipoCuentaId, Balance, Descripcion) 
                                                        values (@Nombre, @TipoCuentaId, @Balance, @Descripcion);
                                                        SELECT SCOPE_IDENTITY();", cuenta);
            cuenta.Id = id;

        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"select Cuentas.Id, Cuentas.Nombre, Balance, tc.Nombre AS TipoCuenta
                                                        from cuentas
                                                        inner join TiposCuentas tc
                                                        on cuentas.TipoCuentaId = tc.id
                                                        where tc.UsuarioId=@UsuarioId
                                                        order by tc.Orden", new { usuarioId });
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(@"select Cuentas.Id, Cuentas.Nombre, Balance, Descripcion, tipoCuentaId
                                                        from cuentas
                                                        inner join TiposCuentas tc
                                                        on cuentas.TipoCuentaId = tc.id
                                                        where tc.UsuarioId=@UsuarioId AND Cuentas.Id=@Id
                                                        ", new { id, usuarioId });

        }

        public async Task<IEnumerable<Cuenta>> listaCuenta(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"select Cuentas. *
                                                        from cuentas
                                                        inner join TiposCuentas tc
                                                        on cuentas.TipoCuentaId = tc.id
                                                        where tc.UsuarioId=@UsuarioId",
                                                                            new { usuarioId });
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            //no quiero retornar nada
            await connection.ExecuteAsync(@"UPDATE Cuentas SET Nombre= @Nombre, Balance= @Balance, TipoCuentaId= @TipoCuentaId, Descripcion= @Descripcion
                                            WHERE Id = @Id", cuenta);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE Cuentas WHERE Id=@Id", new { id });
        }
    }
}
