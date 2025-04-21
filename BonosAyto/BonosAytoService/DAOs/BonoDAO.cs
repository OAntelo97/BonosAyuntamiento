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
        private const string CadenaConexion = "server=DESKTOP-LCFMU2M\\SQLEXPRESS;Database=AytoCoruna;Trusted_Connection=True; TrustServerCertificate=True;";
        public int Insertar(Bono bono)
        {
            using var connection = new SqlConnection(CadenaConexion);
            var sql = "INSERT INTO Bonos(Id, IdBeneficiario, TipoServicio, FechaInicio, FechaCaducidad, Importe, Activados, Canjeados, Caducados, UsuarioMod, FechaMod) VALUES " +
                "(@Id, @IdBeneficiario, @TipoServicio, @FechaInicio, @FechaCaducidad, @Importe, @Activados, @Canjeados, @Caducados, @UsuarioMod, @FechaMod); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            int valorAsignado = connection.QuerySingle<int>(sql, bono);
            return valorAsignado;
        }



        public Bono? Consultar(int id)
        {
            using var connection = new SqlConnection(CadenaConexion);
            var sql = "SELECT * FROM Bonos WHERE Id=@Id";
            return connection.QueryFirstOrDefault<Bono>(sql, new { Id = id });
        }



        public IEnumerable<Bono> Listar()
        {
            using var connection = new SqlConnection(CadenaConexion);
            var sql = "SELECT * FROM Bonos ORDER BY Id ASC";
            return connection.Query<Bono>(sql);
        }



        public bool Actualizar(Bono bono)
        {
            using var connection = new SqlConnection(CadenaConexion);
            var sql = "UPDATE Bonos SET Id=@Id, IdBeneficiario=@IdBeneficiario, TipoServicio=@TipoServicio, FechaInicio=@FechaInicio, FechaCaducidad=@FechaCaducidad," +
                " Importe=@Importe, Activados=@Activados, Canjeados=@Canjeados, Caducados=@Caducados, UsuarioMod=@UsuarioMod, FechaMod=@FechaMod WHERE ID=@Id";
            return connection.Execute(sql, bono) > 0;
        }



        public bool Eliminar(int id)
        {
            using var connection = new SqlConnection(CadenaConexion);
            var sql = "DELETE FROM Bonos WHERE Id=@id";
            return connection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
