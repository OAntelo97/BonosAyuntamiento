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
    public class UsuarioDAO
    {
        private const string conn = "Server=DESKTOP-N3LV49P\\SQLEXPRESS;Database=AytoCoruna;Trusted_Connection=True; TrustServerCertificate=True;";

        public int Insertar(Usuarios user)
        {
            using var connection = new SqlConnection(conn);
            var sql = "INSERT INTO Usuarios (Usuario, Pass, Rol, Email, IdEstablecimiento, UsuarioMod, FechaMod)  VALUES (@Usuario, @Pass, @Rol, @Email, @IdEstablecimiento, @UsuarioMod, @FechaMod);  SELECT CAST(SCOPE_IDENTITY() AS INT);";
            var parameters = new
            {
                user.Usuario,
                user.Pass,
                user.Rol,
                user.Email,
                user.IdEstablecimiento,
                user.UsuarioMod,
                FechaMod=DateTime.Now
            };
            return connection.Execute(sql, parameters);
        }

        public Usuarios? Consultar(int id)
        {

            using var connection = new SqlConnection(conn);
            var sql = "SELECT * FROM Usuarios WHERE Id=@Id;";
            return connection.QueryFirstOrDefault<Usuarios>(sql, new { Id = id });

        }

        public int comprobarUsuario(Usuarios user)
        {
            using var conection = new SqlConnection(conn);
            var sql = "SELECT * FROM Usuarios WHERE Usuario=@Usuario AND Pass = @Pass ;";
            Usuarios usuario = conection.QueryFirstOrDefault<Usuarios>(sql, user);
            return usuario != null? usuario.Id : -1;
        }

        public IEnumerable<Usuarios> Listar()
        {
            using var connection = new SqlConnection(conn);
            var sql = "SELECT * FROM Usuarios";
            return connection.Query<Usuarios>(sql);
        }

        public bool Actualizar(Usuarios user)
        {
            using var connection = new SqlConnection(conn);
            var sql = "UPDATE Usuarios SET Usuario=@Usuario, Pass=@Pass, Rol=@Rol, Email=@Email, IdEstablecimiento=@IdEstablecimiento, UsuarioMod=@UsuarioMod, FechaMod=@FechaMod WHERE Id=@Id;";
            var parameters = new
            {
                user.Id,
                user.Usuario,
                user.Pass,
                user.Rol,
                user.Email,
                user.IdEstablecimiento,
                user.UsuarioMod,
                FechaMod = DateTime.Now
            };
            return connection.Execute(sql, parameters) > 0;

        }
        public bool Eliminar(int id)
        {
            using var connection = new SqlConnection(conn);
            var sql = "DELETE FROM Usuarios WHERE Id=@id;";
            return connection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
