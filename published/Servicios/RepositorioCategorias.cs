using ControlDeFinanzas.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ControlDeFinanzas.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);
        Task<int> Contar(int usuarioId);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId, PaginacionViewModel paginacion);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId);
        Task<Categoria> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly string connectionString;
        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        //configuramos la acción crear
        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"insert into Categorias (Nombre, TipoOperacionId, UsuarioId)
                                                            values (@Nombre, @TipoOperacionId, @UsuarioId)
                                                            select SCOPE_IDENTITY();", categoria);
            categoria.Id = id;
        }
        //añado la paginación. La forma de hacer paginación en SQLSERVER es con OFFSET 1{} cuantos debe saltar 2{} cuantos registros debe de tomar
        //ES OBLIGATORIO PONER UN OREDER BY CUANDO SE ESTA PAGINANDO
        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, PaginacionViewModel paginacion)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(@$"SELECT * 
                                                            FROM Categorias
                                                            WHERE UsuarioId = @UsuarioId
                                                            ORDER BY Nombre
                                                            OFFSET {paginacion.RecordsASaltar} ROWS FETCH NEXT {paginacion.RecordsPorPagina}ROWS ONLY
                                                            ", new { usuarioId });
        }
        //para contar las categorias de un usuario
        public async Task<int> Contar(int usuarioId) 
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.ExecuteScalarAsync<int>(@"select count(*) from Categorias 
                                                    where UsuarioId = @UsuarioId"
                                                    , new {usuarioId });
        }



        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Categoria>(
                                        @"select *
                                        from Categorias 
                                        where UsuarioId=@usuarioId and TipoOperacionId = @TipoOperacionId"
                                        , new { usuarioId, tipoOperacionId });
        }



        public async Task<Categoria> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(@"SELECT * FROM Categorias WHERE  UsuarioId = @UsuarioId 
                                                                        And Id = @Id", new { id, usuarioId });


        }
        public async Task Actualizar(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            //no quiero retornar nada
            await connection.ExecuteAsync(@"UPDATE Categorias SET Nombre= @Nombre, TipoOperacionId=@TipoOperacionId
                                            WHERE Id = @Id", categoria);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                                            DELETE Transacciones1 WHERE CategoriaId = @Id

                                            DELETE Categorias WHERE Id = @Id", new { id });
        }


    }


}
