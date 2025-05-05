using AutoMapper;
using BonosAytoService.DAOs;
using BonosAytoService.DTOs;
using BonosAytoService.Model;

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

        public async Task<bool> Actualizar(EstablecimientoDTO establecimientoDTO)
        {
            var establecimiento = _mapper.Map<Establecimiento>(establecimientoDTO);
            establecimiento.FechaMod = DateTime.Now;
            establecimiento.UsuarioMod = GlobalVariables.usuario.Id;
            return await _dao.Actualizar(establecimiento);
        }

        public async Task<bool> Eliminar(int id)
        {
            return await _dao.Eliminar(id);
        }
    }
}
