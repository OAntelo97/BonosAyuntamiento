using BonosAytoService.Model;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.DAOs
{
    public class BeneficiarioDAO
    {
        public async Task<(int,int?)> Insertar(Beneficiario benf)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "INSERT INTO Beneficiarios (Nombre, PrimerApellido, SegundoApellido, DNI, Direccion, CodigoPostal, Telefono, Email, UsuarioMod, FechaMod) VALUES (@Nombre, @PrimerApellido, @SegundoApellido, @DNI, @Direccion, @CodigoPostal, @Telefono, @Email, @UsuarioMod, @FechaMod); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
           
            try
            {
                return (await connection.QuerySingleAsync<int>(sql, benf), null);
            }
            catch (SqlException ex)
            {
                int error1 = -1;
                int? error2 = null;
                if (ex.Number == 2601)
                {


                    if (ex.Message.Contains("DNI"))
                    {
                        error1 = -2;
                    }
                    if (ex.Message.Contains("Email"))
                    {
                        error2 = -2;
                    }


                }
                return (error1, error2);
            }
        }
        

        public async Task<Beneficiario?> Consultar(int id)
        {
            
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, Nombre, PrimerApellido, SegundoApellido, DNI, Direccion, CodigoPostal, Telefono, Email, UsuarioMod, FechaMod FROM Beneficiarios WHERE Id=@Id;";
            return await connection.QueryFirstOrDefaultAsync<Beneficiario>(sql, new { Id = id });

        }
       
        public async Task<IEnumerable<Beneficiario>> Listar()
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, Nombre, PrimerApellido, SegundoApellido, DNI, Direccion, CodigoPostal, Telefono, Email, UsuarioMod, FechaMod FROM Beneficiarios";
            return await connection.QueryAsync<Beneficiario>(sql);
        }
       
        public async Task<(int, int?)> Actualizar(Beneficiario benf)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "UPDATE Beneficiarios SET Nombre=@Nombre, PrimerApellido=@PrimerApellido, SegundoApellido=@SegundoApellido, DNI=@DNI, Direccion=@Direccion, CodigoPostal=@CodigoPostal, Telefono=@Telefono, Email=@Email, UsuarioMod=@UsuarioMod, FechaMod=@FechaMod WHERE Id=@Id;";
            try
            {
                return (await connection.ExecuteAsync(sql, benf), null);
            }
            catch (SqlException ex)
            {
                int error1 = -1;
                int? error2 = null;
                if (ex.Number == 2601)
                {


                    if (ex.Message.Contains("DNI"))
                    {
                        error1 = -2;
                    }
                    if (ex.Message.Contains("Email"))
                    {
                        error2 = -2;
                    }


                }
                return (error1, error2);
            }
            
        }
        public async Task<bool> Eliminar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "DELETE FROM Beneficiarios WHERE Id=@id;";
            return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
        }
    }
}
