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
        public async Task<int> Insertar(Canjeo canjeo)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "INSERT INTO Canjeos(IdBono, IdEstablecimiento, FechaCanjeo, OpExitosa, DescripcionError, UsuarioMod, FechaMod) VALUES " +
                "(@IdBono, @IdEstablecimiento, @FechaCanjeo, @OpExitosa, @DescripcionError, @UsuarioMod, @FechaMod); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            try
            {
                int valorAsignado = await connection.QuerySingleAsync<int>(sql, canjeo);
                return valorAsignado;
            }
            catch (SqlException ex)
            {
                return -1;
            }
            
        }



        public async Task<Canjeo?> Consultar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, IdBono, IdEstablecimiento, FechaCanjeo, OpExitosa, DescripcionError, UsuarioMod, FechaMod FROM Canjeos WHERE Id=@Id";
            return await connection.QueryFirstOrDefaultAsync<Canjeo>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Canjeo>> ConsultarPorBonos(int idBono)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, IdBono, IdEstablecimiento, FechaCanjeo, OpExitosa, DescripcionError, UsuarioMod, FechaMod FROM Canjeos WHERE IdBono=@IdBono";
            return await connection.QueryAsync<Canjeo>(sql, new { IdBono = idBono });
        }

        public async Task<IEnumerable<Canjeo>> ConsultarPorEstablecimiento(int idEstablcimiento)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, IdBono, IdEstablecimiento, FechaCanjeo, OpExitosa, DescripcionError, UsuarioMod, FechaMod FROM Canjeos WHERE IdEstablecimiento=@IdEstablecimiento";
            return await connection.QueryAsync<Canjeo>(sql, new { IdEstablecimiento = idEstablcimiento });
        }

        public async Task<IEnumerable<Canjeo>> Listar()
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, IdBono, IdEstablecimiento, FechaCanjeo, OpExitosa, DescripcionError, UsuarioMod, FechaMod FROM Canjeos ORDER BY Id ASC";
            return await connection.QueryAsync<Canjeo>(sql);
        }



        public async Task<bool> Actualizar(Canjeo canjeo)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "UPDATE Canjeos SET IdBono=@IdBono, IdEstablecimiento=@IdEstablecimiento, FechaCanjeo=@FechaCanjeo, OpExitosa=@OpExitosa," +
                " DescripcionError=@DescripcionError, UsuarioMod=@UsuarioMod, FechaMod=@FechaMod WHERE ID=@Id";
            try
            {
                return await connection.ExecuteAsync(sql, canjeo) > 0;
            }
            catch (SqlException ex)
            {
                return false;
            }
            
        }



        public async Task<bool> Eliminar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "DELETE FROM Canjeos WHERE Id=@id";
            return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
        }
    }
}
