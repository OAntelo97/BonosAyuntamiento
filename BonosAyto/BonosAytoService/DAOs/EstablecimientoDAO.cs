using Dapper;
using BonosAytoService.Model;
using Microsoft.Data.SqlClient;

namespace BonosAytoService.DAOs
{
    public class EstablecimientoDAO
    {
        public const string cadenaConexion = "Server=DESKTOP-N3LV49P\\SQLEXPRESS;Database=AytoCoruna;Trusted_Connection=True;TrustServerCertificate=True;";
        public int Insertar(Establecimiento establecimiento)
        {
            using var conection = new SqlConnection(cadenaConexion);
            var sql = "INSERT INTO Establecimientos (Nombre, NIF, Direccion, CodigoPostal, Telefono, Email, UsuarioMod, FechaMod) VALUES (@Nombre, @NIF, @Direccion, @CodigoPostal, @Telefono, @Email, @UsuarioMod, @FechaMod); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            int valorAsignado = conection.QuerySingle<int>(sql, establecimiento);
            return valorAsignado;
        }

        public Establecimiento? Consultar(int id)
        {
            using var conection = new SqlConnection(cadenaConexion);
            var sql = "SELECT * FROM Establecimientos WHERE Id = @Id";
            return conection.QueryFirstOrDefault<Establecimiento?>(sql, new { Id = id });
        }

        public IEnumerable<Establecimiento> Listar()
        {
            using var conection = new SqlConnection(cadenaConexion);
            var sql = "SELECT * FROM Establecimientos";
            return conection.Query<Establecimiento>(sql);
        }

        public bool Actualizar(Establecimiento establecimiento)
        {
            using var conection = new SqlConnection(cadenaConexion);
            var sql = "UPDATE Establecimientos SET Nombre = @Nombre, NIF = @NIF, Direccion = @Direccion, CodigoPostal = @CodigoPostal,  Telefono = @Telefono , Email = @Email, UsuarioMod = @UsuarioMod, FechaMod = @FechaMod  WHERE Id = @Id";
            return conection.Execute(sql, establecimiento) > 0;
        }

        public bool Eliminar(int id)
        {
            using var conection = new SqlConnection(cadenaConexion);
            var sql = "DELETE FROM Establecimientos WHERE Id = @Id";
            return conection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
