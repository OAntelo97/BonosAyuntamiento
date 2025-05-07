using System.Text.RegularExpressions;
using AutoMapper;
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
        private readonly CanjeoService canjeoService;
        private readonly BonoService bonoService;
        private readonly BeneficiarioService beneficiarioService;

        public EstablecimientoService()
        {
            _dao = new EstablecimientoDAO();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Establecimiento, EstablecimientoDTO>().ReverseMap();
            }
            );

            _mapper = config.CreateMapper();

            canjeoService = new CanjeoService();
            bonoService = new BonoService();
            beneficiarioService = new BeneficiarioService();
        }

        public async Task<int> Insertar(EstablecimientoDTO establecimientoDTO)
        {
            var establecimiento = _mapper.Map<Establecimiento>(establecimientoDTO);
            establecimiento.FechaMod = DateTime.Now;
            establecimiento.UsuarioMod = GlobalVariables.usuario.Id;
            return await _dao.Insertar(establecimiento);
        }

        public async Task<EstablecimientoDTO?> Consultar(int id)
        {
            var establecimiento = await _dao.Consultar(id);
            return establecimiento == null ? null : _mapper.Map<EstablecimientoDTO>(establecimiento);
        }

        public async Task<IEnumerable<EstablecimientoDTO>> Listar()
        {
            var lista = await _dao.Listar();
            return _mapper.Map<IEnumerable<EstablecimientoDTO>>(lista);
        }

        public async Task<int> Actualizar(EstablecimientoDTO establecimientoDTO)
        {
            var establecimiento = _mapper.Map<Establecimiento>(establecimientoDTO);
            establecimiento.FechaMod = DateTime.Now;
            establecimiento.UsuarioMod = GlobalVariables.usuario.Id;
            return await _dao.Actualizar(establecimiento);
        }

        public async Task<int> Eliminar(int id)
        {
            return await _dao.Eliminar(id);
        }

        public async Task<IEnumerable<BenCanjBonEst>> ConsulatarCanjeos(int id)
        {
            EstablecimientoDTO establecimiento = await Consultar(id);
            IEnumerable<CanjeoDTO> canjeos = await canjeoService.ConsultarPorEstablecimiento(establecimiento.Id);
            List<BenCanjBonEst> res = [];
            foreach (var canjeo in canjeos)
            {
                BonoDTO bono = await bonoService.Consultar(canjeo.IdBono);
                BeneficiarioDTO beneficiario = await beneficiarioService.Consultar(bono.IdBeneficiario);
                BenCanjBonEst benCanjBonEst = new BenCanjBonEst
                {
                    establecimiento = establecimiento,
                    canjeo = canjeo,
                    bono = bono,
                    beneficiario = beneficiario
                };
                res.Add(benCanjBonEst);
            }

            return res;
        }
        public async Task<(int, float)> ConsultarMetricas(int id)
        {
            return await _dao.ConsultarMetricas(id);
        }


        /*GRÁFICOS*/

        //Obtiene la lista de nombres de los Establecimientos ordenados por Id
        public async Task<List<string>> ObtenerTodosLosNombresDeEstablecimientos()
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var query = "SELECT Nombre FROM Establecimientos ORDER BY Id";
            return (await connection.QueryAsync<string>(query)).ToList();
        }


        //Obtiene los datos de los canjeos TODOS los establecimientos, con la comprobacion de si es total o en el trimestre activo (GRÁFICO GENERAL)
        public async Task<EstablecimientoDatosDTO> ObtenerDatosDeTodosLosEstablecimientos(bool soloTrimestreActivo)
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var (fechaInicio, fechaFin) = ObtenerRangoTrimestreEnCurso();
            Console.WriteLine("FECHA DE INNICIOOOOOOOO: " + fechaInicio);
            var query = @"
                SELECT 
                    'Todos' AS NombreEstablecimiento,
                    COUNT(*) AS BonosCanjeados,
                    SUM(CAST(b.Importe AS decimal(10, 2))) AS ImporteTotal
                FROM Canjeos c
                JOIN Bonos b ON c.IdBono = b.Id";

            if (soloTrimestreActivo)
            {
                //si es solo en el trimestre activo
                query += " AND c.FechaCanjeo>=@FechaInicio AND c.FechaCanjeo<=@FechaFin";
            }

            return await connection.QueryFirstOrDefaultAsync<EstablecimientoDatosDTO>(query, new
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }



        //Obtiene los datos de los canjeos TODOS los establecimientos, con la comprobacion de si es total o en el trimestre activo y los muestra por días (GRÁFICO DÍAS SEMANA)
        public async Task<Dictionary<int, (int Bonos, double Importe)>> ObtenerBonosEImportePorDiaSemanaTodos(bool soloTrimestreActivo)
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
                JOIN Establecimientos e ON c.IdEstablecimiento = e.Id";

            if (soloTrimestreActivo)
            {
                //si es solo en el trimestre activo
                query += " AND c.FechaCanjeo>=@fechaInicio AND c.FechaCanjeo<=@fechaFin";
            }

            query += " GROUP BY((DATEPART(WEEKDAY, c.FechaCanjeo) + @@DATEFIRST - 2) % 7 + 1)";


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


        //Obtiene los datos de los canjeos TODOS los establecimientos, con la comprobacion de si es total o en el trimestre activo y los muestra por meses (GRÁFICO MENSUAL)
        public async Task<Dictionary<int, (int Bonos, double Importe)>> ObtenerBonosEImportePorMesTodos(bool soloTrimestreActivo)
        {
            var (fechaInicio, fechaFin) = ObtenerRangoTrimestreEnCurso();

            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());

            var query = @"SELECT 
                MONTH(c.FechaCanjeo) AS Mes,
                COUNT(*) AS TotalBonos,
                SUM(CAST(b.Importe AS decimal(10, 2))) AS TotalImporte
            FROM Canjeos c
            JOIN Bonos b ON c.IdBono = b.Id
            JOIN Establecimientos e ON c.IdEstablecimiento = e.Id";

            if (soloTrimestreActivo)
            {
                //si es solo en el trimestre activo
                query += " AND c.FechaCanjeo>=@fechaInicio AND c.FechaCanjeo<=@fechaFin";
            }

            query += " GROUP BY MONTH(c.FechaCanjeo) ORDER BY Mes";

            var resultados = await connection.QueryAsync<(int Mes, int TotalBonos, double TotalImporte)>(query, new
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });

            var meses = Enumerable.Range(1, 12).ToDictionary(m => m, m => (0, 0.0));
            foreach (var r in resultados)
                meses[r.Mes] = (r.TotalBonos, r.TotalImporte);

            return meses;
        }



        //Obtiene los datos de los canjeos UN establecimiento seleccionado
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


        //Obtiene los datos de los canjeos UN establecimiento seleccionado en el trimestre activo
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
                AND c.FechaCanjeo>=@fechaInicio AND c.FechaCanjeo<=@fechaFin
                GROUP BY e.Nombre";

            return await connection.QueryFirstOrDefaultAsync<EstablecimientoDatosDTO>(query, new
            {
                Nombre = nombre,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });
        }


        //Método que obtiene el trimestre activo
        public (DateTime FechaInicio, DateTime FechaFin) ObtenerRangoTrimestreEnCurso()
        {
            // Ejemplo de lógica.
            var hoy = DateTime.Today;
            int mes = hoy.Month;
            int trimestre = 1;

            switch (mes)
            {
                case 1:
                case 2:
                case 3:
                    trimestre = 1;
                    break;
                case 4:
                case 5:
                case 6:
                    trimestre = 2;
                    break;
                case 7:
                case 8:
                case 9:
                    trimestre = 3;
                    break;
                case 10:
                case 11:
                case 12:
                    trimestre = 4;
                    break;
            }

            DateTime fechaInicio = new DateTime(hoy.Year, (trimestre - 1) * 3 + 1, 1);
            DateTime fechaFin = fechaInicio.AddMonths(3).AddDays(-1);

            return (fechaInicio, fechaFin);
        }




        //Obtiene los datos de los canjeos de UN establecimiento seleccionado, con la comprobacion de si es total o en el trimestre activo y los muestra por días (GRÁFICO DÍAS SEMANA)
        public async Task<Dictionary<int, (int Bonos, double Importe)>> ObtenerBonosEImportePorDiaSemana(string nombre, bool soloTrimestreActivo)
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
                WHERE e.Nombre = @Nombre";

            if (soloTrimestreActivo)
            {
                //si es solo en el trimestre activo
                query += " AND c.FechaCanjeo>=@fechaInicio AND c.FechaCanjeo<=@fechaFin";
            }

            query += " GROUP BY((DATEPART(WEEKDAY, c.FechaCanjeo) + @@DATEFIRST - 2) % 7 + 1)";


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
            ";

            // Si se requiere filtrar por trimestre, añadir la condición correspondiente
            if (soloTrimestreActual)
            {
                query += " AND c.FechaCanjeo>=@fechaInicio AND c.FechaCanjeo<=@fechaFin";
            }

            query += " GROUP BY MONTH(c.FechaCanjeo)";

            var resultados = await connection.QueryAsync<(int Mes, int TotalBonos, double TotalImporte)>(query, new
            {
                Nombre = nombre,
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            });

            // Para asegurarnos de que los meses del trimestre estén en el rango
            if (soloTrimestreActual)
            {
                resultados = resultados.Where(r => r.Mes >= fechaInicio.Month && r.Mes <= fechaFin.Month).ToList();
            }

            var meses = Enumerable.Range(1, 12).ToDictionary(m => m, m => (0, 0.0));
            foreach (var r in resultados)
            {
                meses[r.Mes] = (r.TotalBonos, r.TotalImporte);
            }

            return meses;
        }

    }
}