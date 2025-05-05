using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BonosAytoService.DAOs;
using BonosAytoService.DTOs;
using BonosAytoService.Model;

namespace BonosAytoService.Services
{
    public class CanjeoService
    {
        private readonly CanjeoDAO _dao;
        private readonly IMapper _mapper;


        public CanjeoService()
        {
            _dao = new CanjeoDAO();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Canjeo, CanjeoDTO>().ReverseMap(); //indica que funciona en los dos sentidos, de dto al modelo cliente y viceversa
            }
            );

            _mapper = config.CreateMapper();
        }

        public async Task<int> Insertar(CanjeoDTO canjeoDTO)
        {
            var canjeo = _mapper.Map<Canjeo>(canjeoDTO);
            canjeo.FechaMod = DateTime.Now;
            canjeo.UsuarioMod = GlobalVariables.usuario.Id;
            return await _dao.Insertar(canjeo);
        }



        public async Task<CanjeoDTO?> Consultar(int id)
        {
            var canjeo = await _dao.Consultar(id);
            return canjeo == null ? null : _mapper.Map<CanjeoDTO>(canjeo);
        }

        public async Task<IEnumerable<CanjeoDTO>> ConsultarPorBonos(int idBono)
        {
            var lista = await _dao.ConsultarPorBonos(idBono);
            return _mapper.Map<IEnumerable<CanjeoDTO>>(lista);
        }
        public async Task<IEnumerable<CanjeoDTO>> ConsultarPorEstablecimiento(int idEstablcimiento)
        {
            var lista = await _dao.ConsultarPorEstablecimiento(idEstablcimiento);
            return _mapper.Map<IEnumerable<CanjeoDTO>>(lista);
        }

        public async Task<IEnumerable<CanjeoDTO>> Listar()
        {
            var lista = await _dao.Listar();
            return _mapper.Map<IEnumerable<CanjeoDTO>>(lista);
        }

        public async Task<bool> Actualizar(CanjeoDTO canjeoDTO)
        {
            var canjeo = _mapper.Map<Canjeo>(canjeoDTO);
            canjeo.FechaMod = DateTime.Now;
            canjeo.UsuarioMod = GlobalVariables.usuario.Id;
            return await _dao.Actualizar(canjeo);
        }





        public async Task<bool> Eliminar(int id)
        {
            return await _dao.Eliminar(id);
        }

    }
}
