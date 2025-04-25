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
        public async Task<List<EstablecimientoDatosDTO>> ObtenerDatosPorEstablecimiento()
        {
            using var connection = new SqlConnection(ConexionBD.CadenaDeConexion());
            var query = @"SELECT e.Nombre AS NombreEstablecimiento, COUNT(*) AS BonosCanjeados, SUM(CAST(b.Importe AS decimal(10, 2))) AS ImporteTotal
        FROM Canjeos c JOIN Bonos b ON c.IdBono = b.Id JOIN Establecimientos e ON c.IdEstablecimiento = e.Id
        WHERE c.OpExitosa = 1
        GROUP BY e.Nombre";

            return (await connection.QueryAsync<EstablecimientoDatosDTO>(query)).ToList();
        }

    }
}
