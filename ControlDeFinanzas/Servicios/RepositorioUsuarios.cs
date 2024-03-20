using ControlDeFinanzas.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ControlDeFinanzas.Servicios
{
    public interface IRepositorioUsuarios
    {
        Task<Usuario> BuscarUsuarioPorEamil(string emailNormalizado);
        Task<int> CrearUsuario(Usuario usuario);
    }
    public class RepositorioUsuarios : IRepositorioUsuarios
    {
        private readonly string connectionString;
        public RepositorioUsuarios(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CrearUsuario(Usuario usuario)
        {
            using var connection = new SqlConnection(connectionString);
            var usuarioId = await connection.QuerySingleAsync<int>(@"
                                    insert into Usuarios (Emial, EmailNormalizado, PasswordHash)
                                    values (@Emial, @EmailNormalizado, @PasswordHash);
                                    select scope_identity();", usuario);

            //coloco la info por defecto
            await connection.ExecuteAsync("CrearDatosUsuarioNuevo", new { usuarioId },
                commandType: System.Data.CommandType.StoredProcedure);
            return usuarioId;
        }

        public async Task<Usuario> BuscarUsuarioPorEamil(string emailNormalizado)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<Usuario>(
                "Select * from Usuarios where EmailNormalizado = @EmailNormalizado", new { emailNormalizado });
        }


    }
}
