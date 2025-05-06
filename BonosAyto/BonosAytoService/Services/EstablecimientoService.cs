using System.Net.Http.Headers;
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
                    establecimiento= establecimiento,
                    canjeo= canjeo,
                    bono = bono,
                    beneficiario  = beneficiario
                };
                res.Add(benCanjBonEst);
            }

            return res;
        }
        public async Task<(int,float)> ConsultarMetricas(int id)
        {
            return await _dao.ConsultarMetricas(id);
        }
    }
}
