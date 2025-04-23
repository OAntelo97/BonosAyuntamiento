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

        public int Insertar(CanjeoDTO canjeoDTO)
        {
            var canjeo = _mapper.Map<Canjeo>(canjeoDTO);
            canjeo.FechaMod = DateTime.Now;
            canjeo.UsuarioMod = GlobalVariables.usuario.Id;
            return _dao.Insertar(canjeo);
        }



        public CanjeoDTO? Consultar(int id)
        {
            var canjeo = _dao.Consultar(id);
            return canjeo == null ? null : _mapper.Map<CanjeoDTO>(canjeo);
        }



        public IEnumerable<CanjeoDTO> Listar()
        {
            var lista = _dao.Listar();
            return _mapper.Map<IEnumerable<CanjeoDTO>>(lista);
        }




        public bool Actualizar(CanjeoDTO canjeoDTO)
        {
            var canjeo = _mapper.Map<Canjeo>(canjeoDTO);
            canjeo.FechaMod = DateTime.Now;
            canjeo.UsuarioMod = GlobalVariables.usuario.Id;
            return _dao.Actualizar(canjeo);
        }





        public bool Eliminar(int id)
        {
            return _dao.Eliminar(id);
        }

    }
}
