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
    public class CanjeoDAO
    {
        public int Insertar(Canjeo canjeo)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "INSERT INTO Canjeos(Id, IdBono, IdEstablecimiento, FechaCanjeo, OpExitosa, DescripcionError, UsuarioMod, FechaMod) VALUES " +
                "(@Id, @IdBono, @IdEstablecimiento, @FechaCanjeo, @OpExitosa, @DescripcionError, @UsuarioMod, @FechaMod); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            int valorAsignado = connection.QuerySingle<int>(sql, canjeo);
            return valorAsignado;
        }



        public Canjeo? Consultar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT * FROM Canjeos WHERE Id=@Id";
            return connection.QueryFirstOrDefault<Canjeo>(sql, new { Id = id });
        }



        public IEnumerable<Canjeo> Listar()
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT * FROM Canjeos ORDER BY Id ASC";
            return connection.Query<Canjeo>(sql);
        }



        public bool Actualizar(Canjeo canjeo)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "UPDATE Canjeos SET Id=@Id, IdBono=@IdBono, IdEstablecimiento=@IdEstablecimiento, FechaCanjeo=@FechaCanjeo, OpExitosa=@OpExitosa," +
                " DescripcionError=@DescripcionError, UsuarioMod=@UsuarioMod, FechaMod=@FechaMod WHERE ID=@Id";
            return connection.Execute(sql, canjeo) > 0;
        }



        public bool Eliminar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "DELETE FROM Canjeos WHERE Id=@id";
            return connection.Execute(sql, new { Id = id }) > 0;
        }
    }
}
