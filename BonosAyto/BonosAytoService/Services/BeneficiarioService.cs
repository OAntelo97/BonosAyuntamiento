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

        
        public void Insertar(BeneficiarioDTO ben)
        {
            var bmap = _mapper.Map<Beneficiario>(ben);
<<<<<<< Updated upstream
            bmap.FechaMod = DateTime.Now;
            bmap.UsuarioMod = GlobalVariables.usuario.Id;
            _dao.Insertar(bmap);
=======
            return _dao.Insertar(bmap);
>>>>>>> Stashed changes
        }

        public BeneficiarioDTO? Consultar(int id)
        {
            var c = _dao.Consultar(id);
            return c == null ? null : _mapper.Map<BeneficiarioDTO>(c);
        }

        public IEnumerable<BeneficiarioDTO> Listar()
        {
            var lista = _dao.Listar();
            return lista.Select(_mapper.Map<BeneficiarioDTO>);
        }

        public bool Actualizar(BeneficiarioDTO ben)
        {
            var bmap = _mapper.Map<Beneficiario>(ben);
            bmap.FechaMod = DateTime.Now;
            bmap.UsuarioMod = GlobalVariables.usuario.Id;
            return _dao.Actualizar(bmap);

        }
        public bool Eliminar(int id)
        {
            return _dao.Eliminar(id);
        }
    }
}
