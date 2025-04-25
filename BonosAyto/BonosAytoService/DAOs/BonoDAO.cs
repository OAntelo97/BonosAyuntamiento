using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BonosAytoService.Model;
using BonosAytoService.Services;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BonosAytoService.DAOs
{
    public class BonoDAO
    {
        public int Insertar(Bono bono)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "INSERT INTO Bonos(IdBeneficiario, TipoServicio, FechaInicio, FechaCaducidad, Importe, Activados, Canjeados, Caducados, UsuarioMod, FechaMod) VALUES " +
                "( @IdBeneficiario, @TipoServicio, @FechaInicio, @FechaCaducidad, @Importe, @Activados, @Canjeados, @Caducados, @UsuarioMod, @FechaMod); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            GlobalVariables.usuario = new DTOs.UsuarioDTO();
            GlobalVariables.usuario.Id = 4;

            var parameters = new
            {
                bono.IdBeneficiario,
                bono.TipoServicio,
                bono.FechaInicio,
                bono.FechaCaducidad,
                bono.Importe,
                bono.Activados,
                bono.Canjeados,
                bono.Caducados,
                UsuarioMod = GlobalVariables.usuario.Id,
                FechaMod = DateTime.Now
            };

            int valorAsignado = connection.QuerySingle<int>(sql, parameters);
            return valorAsignado;
        }



        public Bono? Consultar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT * FROM Bonos WHERE Id=@Id";
            return connection.QueryFirstOrDefault<Bono>(sql, new { Id = id });
        }



        public IEnumerable<Bono> Listar()
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT * FROM Bonos ORDER BY Id ASC";
            return connection.Query<Bono>(sql);
        }

        public IEnumerable<Bono> Listar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT * FROM Bonos where IdBeneficiario=@Id ORDER BY FechaInicio ASC";
            return connection.Query<Bono>(sql, new {Id=id });
        }



        public bool Actualizar(Bono bono)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "UPDATE Bonos SET IdBeneficiario=@IdBeneficiario, TipoServicio=@TipoServicio, FechaInicio=@FechaInicio, FechaCaducidad=@FechaCaducidad," +
                " Importe=@Importe, Activados=@Activados, Canjeados=@Canjeados, Caducados=@Caducados, UsuarioMod=@UsuarioMod, FechaMod=@FechaMod WHERE ID=@Id";
            GlobalVariables.usuario = new DTOs.UsuarioDTO();
            GlobalVariables.usuario.Id = 4;

            var parameters = new
            {
                bono.Id,
                bono.IdBeneficiario,
                bono.TipoServicio,
                bono.FechaInicio,
                bono.FechaCaducidad,
                bono.Importe,
                bono.Activados,
                bono.Canjeados,
                bono.Caducados,
                UsuarioMod = GlobalVariables.usuario.Id,
                FechaMod = DateTime.Now
            };

            return connection.Execute(sql, parameters) > 0;
        }



        public bool Eliminar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "DELETE FROM Bonos WHERE Id=@id";
            return connection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
