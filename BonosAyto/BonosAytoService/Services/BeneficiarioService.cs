using AutoMapper;
using BonosAytoService.DAOs;
using BonosAytoService.DTOs;
using BonosAytoService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.Services
{
    public class BeneficiarioService
    {
        private readonly BeneficiarioDAO _dao;
        private readonly IMapper _mapper;

        public BeneficiarioService()
        {
            _dao = new BeneficiarioDAO();
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Beneficiario, BeneficiarioDTO>();
                cfg.CreateMap<BeneficiarioDTO, Beneficiario>();
            });
            _mapper = config.CreateMapper();

        }

        
        public async Task<(int, int?)> Insertar(BeneficiarioDTO ben)
        {
            var bmap = _mapper.Map<Beneficiario>(ben);
            bmap.FechaMod = DateTime.Now;
            bmap.UsuarioMod = GlobalVariables.usuario.Id;
            return await _dao.Insertar(bmap);
        }

        public async Task<BeneficiarioDTO?> Consultar(int id)
        {
            var c = await _dao.Consultar(id);
            return c == null ? null : _mapper.Map<BeneficiarioDTO>(c);
        }

        public async Task<IEnumerable<BeneficiarioDTO>> Listar()
        {
            var lista = await _dao.Listar();
            return lista.Select(_mapper.Map<BeneficiarioDTO>);
        }

        public async Task<(int, int?)> Actualizar(BeneficiarioDTO ben)
        {
            var bmap = _mapper.Map<Beneficiario>(ben);
            bmap.FechaMod = DateTime.Now;
            bmap.UsuarioMod = GlobalVariables.usuario.Id;
            return await _dao.Actualizar(bmap);

        }
        public async Task<bool> Eliminar(int id)
        {
            return await _dao.Eliminar(id);
        }
    }
}
