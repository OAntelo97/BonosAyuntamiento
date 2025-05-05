using System.Text.RegularExpressions;
using AutoMapper;
using Blazorise;
using BonosAytoService.DAOs;
using BonosAytoService.DTOs;
using BonosAytoService.Model;
using Dapper;
using Microsoft.Data.SqlClient;

namespace BonosAytoService.Services
{
    public class EstablecimientoService
    {
        private readonly EstablecimientoDAO _dao;
        private readonly IMapper _mapper;

        public EstablecimientoService()
        {
            _dao = new EstablecimientoDAO();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Establecimiento, EstablecimientoDTO>().ReverseMap();
            }
            );

            _mapper = config.CreateMapper();
        }

        public int Insertar(EstablecimientoDTO establecimientoDTO)
        {
            var establecimiento = _mapper.Map<Establecimiento>(establecimientoDTO);
            establecimiento.FechaMod = DateTime.Now;
            establecimiento.UsuarioMod = GlobalVariables.usuario.Id;
            return _dao.Insertar(establecimiento);
        }

        public EstablecimientoDTO? Consultar(int id)
        {
            var establecimiento = _dao.Consultar(id);
            return establecimiento == null ? null : _mapper.Map<EstablecimientoDTO>(establecimiento);
        }

        public IEnumerable<EstablecimientoDTO> Listar()
        {
            var lista = _dao.Listar();
            return _mapper.Map<IEnumerable<EstablecimientoDTO>>(lista);
        }

        public bool Actualizar(EstablecimientoDTO establecimientoDTO)
        {
            var establecimiento = _mapper.Map<Establecimiento>(establecimientoDTO);
            establecimiento.FechaMod = DateTime.Now;
            establecimiento.UsuarioMod = GlobalVariables.usuario.Id;
            return _dao.Actualizar(establecimiento);
        }

        public bool Eliminar(int id)
        {
            return _dao.Eliminar(id);
        }



        /*GRÁFICOS*/
        public async Task<List<string>> ObtenerTodosLosNombresDeEstablecimientos()
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var query = "SELECT Nombre FROM Establecimientos ORDER BY Id";
            return (await connection.QueryAsync<string>(query)).ToList();
        }
        public async Task<EstablecimientoDatosDTO> ObtenerDatosDeTodosLosEstablecimientos()
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());

            var query = @"
                SELECT 
                    'Todos' AS NombreEstablecimiento,
                    COUNT(*) AS BonosCanjeados,
                    SUM(CAST(b.Importe AS decimal(10, 2))) AS ImporteTotal
                FROM Canjeos c
                JOIN Bonos b ON c.IdBono = b.Id";

            return await connection.QueryFirstOrDefaultAsync<EstablecimientoDatosDTO>(query);
        }
        public async Task<Dictionary<int, (int Bonos, double Importe)>> ObtenerBonosEImportePorDiaSemanaTodos(bool soloTrimestreActual)
        {
            var (fechaInicio, fechaFin) = ObtenerRangoTrimestreEnCurso();

            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());

            var query = @"
                SELECT 
                    ((DATEPART(WEEKDAY, c.FechaCanjeo) + @@DATEFIRST - 2) % 7 + 1) AS DiaSemana,
                    COUNT(*) AS TotalBonos,
                    SUM(CAST(b.Importe AS decimal(10, 2))) AS TotalImporte 
                FROM Canjeos c
                JOIN Bonos b ON c.IdBono = b.Id
                JOIN Establecimientos e ON c.IdEstablecimiento = e.Id
                WHERE 1=1
                " + (soloTrimestreActual ? "AND b.FechaInicio >= @FechaInicio AND b.FechaCaducidad <= @FechaFin" : "") + @"
                GROUP BY ((DATEPART(WEEKDAY, c.FechaCanjeo) + @@DATEFIRST - 2) % 7 + 1)";


            var resultados = await connection.QueryAsync<(int DiaSemana, int TotalBonos, double TotalImporte)>(query, new
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });

            var diasSemana = Enumerable.Range(1, 7).ToDictionary(d => d, d => (0, 0.0));
            foreach (var r in resultados)
                diasSemana[r.DiaSemana] = (r.TotalBonos, r.TotalImporte);

            return diasSemana;
        }
        public async Task<Dictionary<int, (int Bonos, double Importe)>> ObtenerBonosEImportePorMesTodos(bool soloTrimestreActual)
        {
            var (fechaInicio, fechaFin) = ObtenerRangoTrimestreEnCurso();

            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());

            var query = @"
                SELECT 
                    MONTH(c.FechaCanjeo) AS Mes,
                    COUNT(*) AS TotalBonos,
                    SUM(CAST(b.Importe AS decimal(10, 2))) AS TotalImporte
                FROM Canjeos c
                JOIN Bonos b ON c.IdBono = b.Id
                JOIN Establecimientos e ON c.IdEstablecimiento = e.Id
                " + (soloTrimestreActual ? "AND b.FechaInicio >= @FechaInicio AND b.FechaCaducidad <= @FechaFin" : "") + @"
                GROUP BY MONTH(c.FechaCanjeo)";

            var resultados = await connection.QueryAsync<(int Mes, int TotalBonos, double TotalImporte)>(query, new
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });

            // Verificar los resultados para todos los meses
            foreach (var r in resultados)
            {
                Console.WriteLine($"Mes: {r.Mes}, Bonos: {r.TotalBonos}, Importe: {r.TotalImporte}");
            }

            var meses = Enumerable.Range(1, 12).ToDictionary(m => m, m => (0, 0.0));
            foreach (var r in resultados)
                meses[r.Mes] = (r.TotalBonos, r.TotalImporte);

            return meses;
        }




        public async Task<EstablecimientoDatosDTO> ObtenerDatosPorEstablecimiento(string nombre)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());

            var query = @"
                SELECT 
                    e.Nombre AS NombreEstablecimiento,
                    COUNT(*) AS BonosCanjeados,
                    SUM(CAST(b.Importe AS decimal(10, 2))) AS ImporteTotal
                FROM Canjeos c
                JOIN Bonos b ON c.IdBono = b.Id
                JOIN Establecimientos e ON c.IdEstablecimiento = e.Id
                WHERE e.Nombre = @Nombre
                GROUP BY e.Nombre";

            return await connection.QueryFirstOrDefaultAsync<EstablecimientoDatosDTO>(query, new { Nombre = nombre });
        }

        public async Task<EstablecimientoDatosDTO> ObtenerDatosUltimoTrimestrePorEstablecimiento(string nombre)
        {
            var (fechaInicio, fechaFin) = ObtenerRangoTrimestreEnCurso();

            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());

            var query = @"
                SELECT 
                    e.Nombre AS NombreEstablecimiento,
                    COUNT(*) AS BonosCanjeados,
                    SUM(CAST(b.Importe AS decimal(10, 2))) AS ImporteTotal
                FROM Canjeos c
                JOIN Bonos b ON c.IdBono = b.Id
                JOIN Establecimientos e ON c.IdEstablecimiento = e.Id
                WHERE e.Nombre = @Nombre
                AND b.FechaInicio >= @FechaInicio AND b.FechaCaducidad <= @FechaFin
                GROUP BY e.Nombre";

            return await connection.QueryFirstOrDefaultAsync<EstablecimientoDatosDTO>(query, new
            {
                Nombre = nombre,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }

        private (DateTime fechaInicio, DateTime fechaFin) ObtenerRangoTrimestreEnCurso()
        {
            var fechaActual = DateTime.Now;
            int mesActual = fechaActual.Month;
            int trimestreActual = (int)Math.Ceiling(mesActual / 3.0);
            int mesInicio = (trimestreActual - 1) * 3 + 1;

            var fechaInicio = new DateTime(fechaActual.Year, mesInicio, 1);
            var fechaFin = fechaInicio.AddMonths(3).AddDays(-1); // Fin del trimestre

            return (fechaInicio, fechaFin);
        }

        public async Task<Dictionary<int, (int Bonos, double Importe)>> ObtenerBonosEImportePorDiaSemana(string nombre, bool soloTrimestreActual)
        {
            var (fechaInicio, fechaFin) = ObtenerRangoTrimestreEnCurso();

            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());

            //con el datepart hacemos que los días sean en formato español (1=lunes,2=martes..) porque sqlserver por defecto devuelve el formato ingles (1=domingo..)
            var query = @"
                SELECT 
                    ((DATEPART(WEEKDAY, c.FechaCanjeo) + @@DATEFIRST - 2) % 7 + 1) AS DiaSemana,
                    COUNT(*) AS TotalBonos,
                    SUM(CAST(b.Importe AS decimal(10, 2))) AS TotalImporte
                FROM Canjeos c
                JOIN Bonos b ON c.IdBono = b.Id
                JOIN Establecimientos e ON c.IdEstablecimiento = e.Id
                WHERE e.Nombre = @Nombre
                " + (soloTrimestreActual ? "AND b.FechaInicio >= @FechaInicio AND b.FechaCaducidad <= @FechaFin" : "") + @"
                GROUP BY ((DATEPART(WEEKDAY, c.FechaCanjeo) + @@DATEFIRST - 2) % 7 + 1)";


            var resultados = await connection.QueryAsync<(int DiaSemana, int TotalBonos, double TotalImporte)>(query, new
            {
                Nombre = nombre,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });

            // Mapear de 1 (lunes) a 7 (domingo)
            var diasSemana = Enumerable.Range(1, 7).ToDictionary(d => d, d => (0, 0.0));
            foreach (var r in resultados)
                diasSemana[r.DiaSemana] = (r.TotalBonos, r.TotalImporte);

            return diasSemana;
        }



        public async Task<Dictionary<int, (int Bonos, double Importe)>> ObtenerBonosEImportePorMes(string nombre, bool soloTrimestreActual)
        {
            var (fechaInicio, fechaFin) = ObtenerRangoTrimestreEnCurso();

            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());

            var query = @"
                SELECT 
                    MONTH(c.FechaCanjeo) AS Mes,
                    COUNT(*) AS TotalBonos,
                    SUM(CAST(b.Importe AS decimal(10, 2))) AS TotalImporte
                FROM Canjeos c
                JOIN Bonos b ON c.IdBono = b.Id
                JOIN Establecimientos e ON c.IdEstablecimiento = e.Id
                WHERE e.Nombre = @Nombre
                " + (soloTrimestreActual ? "AND b.FechaInicio >= @FechaInicio AND b.FechaCaducidad <= @FechaFin" : "") + @"
                GROUP BY MONTH(c.FechaCanjeo)";

            var resultados = await connection.QueryAsync<(int Mes, int TotalBonos, double TotalImporte)>(query, new
            {
                Nombre = nombre,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });

            var meses = Enumerable.Range(1, 12).ToDictionary(m => m, m => (0, 0.0));
            foreach (var r in resultados)
            {
                Console.WriteLine($"Mes: {r.Mes}, Bonos: {r.TotalBonos}, Importe: {r.TotalImporte}");
                meses[r.Mes] = (r.TotalBonos, r.TotalImporte);
            }


            return meses;
        }

    }
}
