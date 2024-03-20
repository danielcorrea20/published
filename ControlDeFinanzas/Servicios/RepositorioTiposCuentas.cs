using ControlDeFinanzas.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ControlDeFinanzas.Servicios
{

    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId, int id = 0);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string ConnectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //uso programación asincrona
        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(ConnectionString);
            var id = await connection.QuerySingleAsync<int>("TiposCuentas_Insertar",
                                                            new
                                                            {
                                                                usuarioId = tipoCuenta.UsuarioId,
                                                                nombre = tipoCuenta.Nombre
                                                            },
                                                            commandType: System.Data.CommandType.StoredProcedure);
            tipoCuenta.Id = id;
        }
        //Metodo para evitar que un usuario tenga un tipo de cuenta repetido
        public async Task<bool> Existe(string nombre, int usuarioId, int id=0)
        {
            using var connection = new SqlConnection(ConnectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1
                                                                        FROM tiposCuentas
                                                                        WHERE nombre=@Nombre AND UsuarioId = @UsuarioId AND Id <> @id;",
                                                                        new { nombre, usuarioId, id });
            return existe == 1;
        }
        //muestro todos los tipos cuentas que hay. Lo hago de forma asincrona, el task es el dato que reciviré
        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(ConnectionString);
            return await connection.QueryAsync<TipoCuenta>(@$"SELECT Id, Nombre, Orden
                                                            FROM TiposCuentas
                                                            WHERE UsuarioId = @UsuarioId
                                                           
                                                            ORDER BY Orden", new { usuarioId });
        }
        //Actualizo el tipo cuenta. Lo hago de forma asincrona, el task es el dato que reciviré
        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas
                                            SET Nombre=@Nombre
                                            WHERE Id = @Id", tipoCuenta);
        }
        //Consigo el tipo de cuenta por Id del tipo cuenta para ello necesito el id y el usuarioId, para evitar que u usuario no aceda a una uenta que no es l suya. Lo hago de forma asincrona, el task es el dato que reciviré
        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(ConnectionString);
            //FirstOrDefaultAsync para que me devurlva el primer resultado o uno por defecto si no hay ninguno. Lo maeo todo a TipoCuenta
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden
                                                                            FROM TiposCuentas
                                                                            WHERE Id = @Id AND UsuarioId = @UsuarioId", new { id, usuarioId });
        }

        //Creo el método de borrar
        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(ConnectionString);
            //FirstOrDefaultAsync para que me devurlva el primer resultado o uno por defecto si no hay ninguno. Lo maeo todo a TipoCuenta
            await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"DELETE TiposCuentas
                                                                        WHERE Id = @Id", new { id });
        }

        //guardo los cambios al mover los tipos cuentas
        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentasOrdenados)
        {
            var query = "UPDATE TiposCuentas SET Orden = @Orden WHERE Id = @Id;";
            using var connection = new SqlConnection(ConnectionString);
            await connection.ExecuteAsync(query, tipoCuentasOrdenados);
        }


    }
}
