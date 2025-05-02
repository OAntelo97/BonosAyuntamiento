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
    public class BonoService
    {
        private readonly BonoDAO _dao;
        private readonly IMapper _mapper;


        public BonoService()
        {
            _dao = new BonoDAO();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Bono, BonoDTO>().ReverseMap(); //indica que funciona en los dos sentidos, de dto al modelo cliente y viceversa
            }
            );

            _mapper = config.CreateMapper();
        }

        public async Task<int> Insertar(BonoDTO bonoDTO)
        {
            var bono = _mapper.Map<Bono>(bonoDTO);
            bono.FechaMod = DateTime.Now;
            bono.UsuarioMod = GlobalVariables.usuario.Id;
            return await _dao.Insertar(bono);
        }



        public async Task<BonoDTO?> Consultar(int id)
        {
            var bono = await _dao.Consultar(id);
            return bono == null ? null : _mapper.Map<BonoDTO>(bono);
        }

        public async Task<IEnumerable<BonoDTO>> ConsultarPorBeneficiario(int idBeneficiario)
        {
            var lista = await _dao.ConsultarPorBeneficiario(idBeneficiario);
            return _mapper.Map<IEnumerable<BonoDTO>>(lista);
        }

        public async Task<IEnumerable<BonoDTO>> Listar()
        {
            var lista = await _dao.Listar();
            return _mapper.Map<IEnumerable<BonoDTO>>(lista);
        }
        public async Task<IEnumerable<BonoDTO>> ListarFiltT(int Id)
        {
            var lista = (await _dao.ListarFiltT(Id)).OrderBy(b => b.FechaInicio);
            var mapped = _mapper.Map<IEnumerable<BonoDTO>>(lista);
            return mapped;
        }


        public async Task<bool> Actualizar(BonoDTO bonoDTO)
        {
            var bono = _mapper.Map<Bono>(bonoDTO);
            bono.FechaMod = DateTime.Now;
            bono.UsuarioMod = GlobalVariables.usuario.Id;
            return await _dao.Actualizar(bono);
        }


        public async Task<bool> Eliminar(int id)
        {
            return await _dao.Eliminar(id);
        }

    }
}
