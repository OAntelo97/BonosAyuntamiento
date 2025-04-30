using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BonosAytoService.Model;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BonosAytoService.DAOs
{
    public class BonoDAO
    {
        public int Insertar(Bono bono)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "INSERT INTO Bonos(Id, IdBeneficiario, TipoServicio, FechaInicio, FechaCaducidad, Importe, Activados, Canjeados, Caducados, UsuarioMod, FechaMod) VALUES " +
                "(@Id, @IdBeneficiario, @TipoServicio, @FechaInicio, @FechaCaducidad, @Importe, @Activados, @Canjeados, @Caducados, @UsuarioMod, @FechaMod); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            int valorAsignado = connection.QuerySingle<int>(sql, bono);
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
            return connection.Query<Bono>(sql, new { Id = id });
        }
        public IEnumerable<Bono> ListarFiltT(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());

            var sqlFecha = "SELECT MAX(FechaCaducidad) FROM Bonos WHERE IdBeneficiario = @Id";
            var maxFecha = connection.ExecuteScalar<DateTime?>(sqlFecha, new { Id = id });

            if (maxFecha == null)
                return Enumerable.Empty<Bono>();

            var f2 = maxFecha.Value;
            var f1 = f2.AddMonths(-3);

            var sqlBonos = "SELECT * FROM Bonos WHERE IdBeneficiario = @Id AND FechaCaducidad BETWEEN @F1 AND @F2 ORDER BY FechaInicio ASC";

            return connection.Query<Bono>(sqlBonos, new { Id = id, F1 = f1, F2 = f2 });
        }



        public bool Actualizar(Bono bono)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "UPDATE Bonos SET Id=@Id, IdBeneficiario=@IdBeneficiario, TipoServicio=@TipoServicio, FechaInicio=@FechaInicio, FechaCaducidad=@FechaCaducidad," +
                " Importe=@Importe, Activados=@Activados, Canjeados=@Canjeados, Caducados=@Caducados, UsuarioMod=@UsuarioMod, FechaMod=@FechaMod WHERE ID=@Id";
            return connection.Execute(sql, bono) > 0;
        }



        public bool Eliminar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "DELETE FROM Bonos WHERE Id=@id";
            return connection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
