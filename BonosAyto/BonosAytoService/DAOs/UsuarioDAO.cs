using BonosAytoService.Model;
using BonosAytoService.Utils;
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
        private const string conn = "server=DESKTOP-LCFMU2M\\SQLEXPRESS;Database=AytoCoruna;Trusted_Connection=True; TrustServerCertificate=True;";

        public int Insertar(Usuarios user)
        {
            using var connection = new SqlConnection(conn);
            var sql = "INSERT INTO Usuarios (Usuario, Pass, Rol, Email, IdEstablecimiento, UsuarioMod, FechaMod)  VALUES (@Usuario, @Pass, @Rol, @Email, @IdEstablecimiento, @UsuarioMod, @FechaMod);  SELECT CAST(SCOPE_IDENTITY() AS INT);";
            user.Pass = HashUtil.ObtenerHashSHA256(user.Pass);
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
