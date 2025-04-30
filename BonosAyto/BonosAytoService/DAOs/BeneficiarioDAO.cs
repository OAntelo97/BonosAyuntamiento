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
        public int  Insertar(Beneficiario benf)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "INSERT INTO Beneficiarios (Nombre, PrimerApellido, SegundoApellido, DNI, Direccion, CodigoPostal, Telefono, Email, UsuarioMod, FechaMod) VALUES (@Nombre, @PrimerApellido, @SegundoApellido, @DNI, @Direccion, @CodigoPostal, @Telefono, @Email, @UsuarioMod, @FechaMod);";

            var parameters = new
            {
                benf.Nombre,
                benf.PrimerApellido,
                benf.SegundoApellido,
                benf.DNI,
                benf.Direccion,
                benf.CodigoPostal,
                benf.Telefono,
                benf.Email,
                benf.UsuarioMod,
                FechaMod=DateTime.Now
            };
           return  connection.Execute(sql, parameters);
        }

        public Beneficiario? Consultar(int id)
        {
            
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT * FROM Beneficiarios WHERE Id=@Id;";
            return connection.QueryFirstOrDefault<Beneficiario>(sql, new { Id = id });

        }
       
        public IEnumerable<Beneficiario> Listar()
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT * FROM Beneficiarios";
            return connection.Query<Beneficiario>(sql);
        }
       
        public bool Actualizar(Beneficiario benf)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "UPDATE Beneficiarios SET Nombre=@Nombre, PrimerApellido=@PrimerApellido, SegundoApellido=@SegundoApellido, DNI=@DNI, Direccion=@Direccion, CodigoPostal=@CodigoPostal, Telefono=@Telefono, Email=@Email, UsuarioMod=@UsuarioMod, FechaMod=@FechaMod WHERE Id=@Id;";
            var parameters = new
            {
                benf.Id,
                benf.Nombre,
                benf.PrimerApellido,
                benf.SegundoApellido,
                benf.DNI,
                benf.Direccion,
                benf.CodigoPostal,
                benf.Telefono,
                benf.Email,
                benf.UsuarioMod,
                FechaMod=DateTime.Now
            };
            return connection.Execute(sql, parameters) > 0;
        }
        public bool Eliminar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "DELETE FROM Beneficiarios WHERE Id=@id;";
            return connection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
