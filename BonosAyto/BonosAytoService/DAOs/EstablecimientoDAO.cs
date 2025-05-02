using Dapper;
using BonosAytoService.Model;
using Microsoft.Data.SqlClient;

namespace BonosAytoService.DAOs
{
    public class EstablecimientoDAO
    {
        public async Task<int> Insertar(Establecimiento establecimiento)
        {
            using var conection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "INSERT INTO Establecimientos (Nombre, NIF, Direccion, CodigoPostal, Telefono, Email, UsuarioMod, FechaMod) VALUES (@Nombre, @NIF, @Direccion, @CodigoPostal, @Telefono, @Email, @UsuarioMod, @FechaMod); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
            try
            {
                int valorAsignado = await conection.QuerySingleAsync<int>(sql, establecimiento);
                return valorAsignado;
            }
            catch (SqlException ex)
            {
                return -1;
            }
            
        }

        public async Task<Establecimiento?> Consultar(int id)
        {
            using var conection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Nombre, NIF, Direccion, CodigoPostal, Telefono, Email, UsuarioMod, FechaMod FROM Establecimientos WHERE Id = @Id";
            return await conection.QueryFirstOrDefaultAsync<Establecimiento?>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Establecimiento>> Listar()
        {
            using var conection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Nombre, NIF, Direccion, CodigoPostal, Telefono, Email, UsuarioMod, FechaMod FROM Establecimientos";
            return await conection.QueryAsync<Establecimiento>(sql);
        }

        public async Task<bool> Actualizar(Establecimiento establecimiento)
        {
            using var conection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "UPDATE Establecimientos SET Nombre = @Nombre, NIF = @NIF, Direccion = @Direccion, CodigoPostal = @CodigoPostal,  Telefono = @Telefono , Email = @Email, UsuarioMod = @UsuarioMod, FechaMod = @FechaMod  WHERE Id = @Id";
            try
            {
                return await conection.ExecuteAsync(sql, establecimiento) > 0;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            using var conection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "DELETE FROM Establecimientos WHERE Id = @Id";
            return await conection.ExecuteAsync(sql, new { Id = id }) > 0;
        }
    }
}
