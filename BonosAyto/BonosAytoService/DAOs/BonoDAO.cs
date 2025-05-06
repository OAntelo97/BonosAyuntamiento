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
        public async Task<int> Insertar(Bono bono)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "INSERT INTO Bonos (IdBeneficiario, TipoServicio, FechaInicio, FechaCaducidad, Importe, Activados, Canjeados, Caducados, UsuarioMod, FechaMod) VALUES " +
                "(@IdBeneficiario, @TipoServicio, @FechaInicio, @FechaCaducidad, @Importe, @Activados, @Canjeados, @Caducados, @UsuarioMod, @FechaMod); SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
            try
            {
                int valorAsignado = await connection.QuerySingleAsync<int>(sql, bono);
                return valorAsignado;
            }
            catch (SqlException ex)
            {
                return -1;
            }
        }



        public async Task<Bono?> Consultar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, IdBeneficiario, TipoServicio, FechaInicio, FechaCaducidad, Importe, Activados, Canjeados, Caducados, UsuarioMod, FechaMod FROM Bonos WHERE Id=@Id";
            return await connection.QueryFirstOrDefaultAsync<Bono>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Bono>> ConsultarPorBeneficiario(int idBeneficiario)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, IdBeneficiario, TipoServicio, FechaInicio, FechaCaducidad, Importe, Activados, Canjeados, Caducados, UsuarioMod, FechaMod FROM Bonos WHERE IdBeneficiario=@IdBeneficiario ORDER BY FechaInicio ASC";
            return await connection.QueryAsync<Bono>(sql, new { IdBeneficiario = idBeneficiario });
        }



        public async Task<IEnumerable<Bono>> Listar()
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "SELECT Id, IdBeneficiario, TipoServicio, FechaInicio, FechaCaducidad, Importe, Activados, Canjeados, Caducados, UsuarioMod, FechaMod FROM Bonos ORDER BY FechaInicio ASC";
            return await connection.QueryAsync<Bono>(sql);
        }

        public async Task<IEnumerable<Bono>> ListarFiltT(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());

            var sqlFecha = "SELECT MAX(FechaCaducidad) FROM Bonos WHERE IdBeneficiario = @Id";
            var maxFecha = await connection.ExecuteScalarAsync<DateTime?>(sqlFecha, new { Id = id });

            if (maxFecha == null)
                return Enumerable.Empty<Bono>();

            var f2 = maxFecha.Value;
            var f1 = f2.AddMonths(-3);

            var sqlBonos = "SELECT Id, IdBeneficiario, TipoServicio, FechaInicio, FechaCaducidad, Importe, Activados, Canjeados, Caducados, UsuarioMod, FechaMod FROM Bonos WHERE IdBeneficiario = @Id AND FechaCaducidad BETWEEN @F1 AND @F2 ORDER BY FechaInicio ASC";

            return await connection.QueryAsync<Bono>(sqlBonos, new { Id = id, F1 = f1, F2 = f2 });
        }



        public async Task<bool> Actualizar(Bono bono)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "UPDATE Bonos SET IdBeneficiario=@IdBeneficiario, TipoServicio=@TipoServicio, FechaInicio=@FechaInicio, FechaCaducidad=@FechaCaducidad," +
                " Importe=@Importe, Activados=@Activados, Canjeados=@Canjeados, Caducados=@Caducados, UsuarioMod=@UsuarioMod, FechaMod=@FechaMod WHERE ID=@Id";
            
            try
            {
                return await connection.ExecuteAsync(sql, bono) > 0;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }



        public async Task<bool> Eliminar(int id)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var sql = "DELETE FROM Bonos WHERE Id=@id";
            return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
        }
    }
}
