using Dapper;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using BonosAytoService.Models;

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
                return await conection.QuerySingleAsync<int>(sql, establecimiento);
            }
            catch (SqlException ex)
            {
                int error1 = -1;
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    if (ex.Message.Contains("NIF"))
                    {
                        error1 = -2;
                    }
                    if (ex.Message.Contains("Email"))
                    {
                        error1 = -3;
                    }
                }
                return error1;
            }

        }

        public async Task<Establecimiento?> Consultar(int id)
        {
            using var conection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, Nombre, NIF, Direccion, CodigoPostal, Telefono, Email, UsuarioMod, FechaMod FROM Establecimientos WHERE Id = @Id";
            return await conection.QueryFirstOrDefaultAsync<Establecimiento?>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Establecimiento>> Listar()
        {
            using var conection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, Nombre, NIF, Direccion, CodigoPostal, Telefono, Email, UsuarioMod, FechaMod FROM Establecimientos";
            return await conection.QueryAsync<Establecimiento>(sql);
        }

        public async Task<int> Actualizar(Establecimiento establecimiento)
        {
            using var conection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "UPDATE Establecimientos SET Nombre = @Nombre, NIF = @NIF, Direccion = @Direccion, CodigoPostal = @CodigoPostal,  Telefono = @Telefono , Email = @Email, UsuarioMod = @UsuarioMod, FechaMod = @FechaMod  WHERE Id = @Id";

            try
            {
                return await conection.ExecuteAsync(sql, establecimiento);
            }
            catch (SqlException ex)
            {
                int error1 = -1;
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    if (ex.Message.Contains("NIF"))
                    {
                        error1 = -2;
                    }
                    if (ex.Message.Contains("Email"))
                    {
                        error1 = -3;
                    }
                }
                return error1;
            }
        }

        public async Task<int> Eliminar(int id)
        {
            using var conection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "DELETE FROM Establecimientos WHERE Id = @Id";
            try
            {
                return await conection.ExecuteAsync(sql, new { Id = id });
            }
            catch (SqlException ex)
            {
                int error1 = -1;
                if (ex.Number == 547)
                {
                    if (ex.Message.Contains("Usuario"))
                    {
                        error1 = -2;
                    }
                    else if (ex.Message.Contains("Canjeos"))
                    {
                        error1 = -3;
                    }
                }
                return (error1);
            }
        }

        public async Task<(int, float)> ConsultarMetricas(int id)
        {
            using var conection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "select count(c.Id), Sum(b.Importe) from Bonos as b JOIN Canjeos as c on c.IdBono = b.Id Where c.IdEstablecimiento = @Id";
            return await conection.QueryFirstOrDefaultAsync<(int, float)>(sql, new { Id = id });
        }

    }
}
