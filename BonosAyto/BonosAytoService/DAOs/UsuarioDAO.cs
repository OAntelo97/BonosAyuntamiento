using BonosAytoService.Model;
using BonosAytoService.Services;
using BonosAytoService.Utils;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BonosAytoService.DAOs
{
    public class UsuarioDAO
    {
        public async Task<int> Insertar(Usuarios user)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "INSERT INTO Usuarios (Usuario, Pass, Rol, Email, IdEstablecimiento, UsuarioMod, FechaMod)  VALUES (@Usuario, @Pass, @Rol, @Email, @IdEstablecimiento, @UsuarioMod, @FechaMod);  SELECT CAST(SCOPE_IDENTITY() AS INT);";
            user.Pass = HashUtil.ObtenerHashSHA256(user.Pass);

            try
            {
                return await connection.ExecuteAsync(sql, user);
            }
            catch (SqlException ex){
                int error = -1;
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    if (ex.Message.Contains("Email"))
                    {
                        error = -3;
                    }
                    else if (ex.Message.Contains("Usuario"))
                    {
                        error = -2;
                    }
                }
                return error;
            }
            
        }

        public async Task<Usuarios?> Consultar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, Usuario, Pass, Rol, Email, IdEstablecimiento, UsuarioMod, FechaMod FROM Usuarios WHERE Id=@Id;";
            return await connection.QueryFirstOrDefaultAsync<Usuarios>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Usuarios>> ConsultarPorEstablecimiento(int idEstablecimiento)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, Usuario, Pass, Rol, Email, IdEstablecimiento, UsuarioMod, FechaMod FROM Usuarios WHERE IdEstablecimiento=@IdEstablecimiento;";
            return await connection.QueryAsync<Usuarios>(sql, new { IdEstablecimiento = idEstablecimiento });
        }

        public async Task<int> comprobarUsuario(Usuarios user)
        {
            using var conection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, Usuario, Pass, Rol, Email, IdEstablecimiento, UsuarioMod, FechaMod FROM Usuarios WHERE Usuario = @Usuario AND Pass = @Pass ;";
            user.Pass = HashUtil.ObtenerHashSHA256(user.Pass);
            Usuarios? usuario = await conection.QueryFirstOrDefaultAsync<Usuarios>(sql, user);
            return usuario != null ? usuario.Id : -1;
        }

        public async Task<IEnumerable<Usuarios>> Listar()
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, Usuario, Pass, Rol, Email, IdEstablecimiento, UsuarioMod, FechaMod FROM Usuarios";
            return await connection.QueryAsync<Usuarios>(sql);
        }

        public async Task<int> Actualizar(Usuarios user)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "UPDATE Usuarios SET Usuario=@Usuario, Pass=@Pass, Rol=@Rol, Email=@Email, IdEstablecimiento=@IdEstablecimiento, UsuarioMod=@UsuarioMod, FechaMod=@FechaMod WHERE Id=@Id;";
            
            try
            {
                return await connection.ExecuteAsync(sql, user) > 0? 0:-1;
            }
            catch (SqlException ex)
            {
                int error = -1;
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    if (ex.Message.Contains("Email"))
                    {
                        error = -3;
                    }
                    else if (ex.Message.Contains("Usuario"))
                    {
                        error = -2;
                    }
                }
                return error;
            }
            

        }
        public async Task<bool> Eliminar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "DELETE FROM Usuarios WHERE Id=@id;";
            return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
        }
    }
}
