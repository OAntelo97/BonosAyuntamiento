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

        public int Insertar(BonoDTO bonoDTO)
        {
            var bono = _mapper.Map<Bono>(bonoDTO);

            bono.UsuarioMod = GlobalVariables.usuario.Id;
            return _dao.Insertar(bono);
        }



        public BonoDTO? Consultar(int id)
        {
            var bono = _dao.Consultar(id);
            return bono == null ? null : _mapper.Map<BonoDTO>(bono);
        }



        public IEnumerable<BonoDTO> Listar()
        {
            var lista = _dao.Listar();
            return _mapper.Map<IEnumerable<BonoDTO>>(lista);
        }
        public IEnumerable<BonoDTO> Listar( int Id)
        {
            var lista = _dao.Listar(Id).OrderBy(b => b.FechaInicio);
            var mapped = _mapper.Map<IEnumerable<BonoDTO>>(lista);
            return mapped;
        }


        public bool Actualizar(BonoDTO bonoDTO)
        {
            var bono = _mapper.Map<Bono>(bonoDTO);

            bono.UsuarioMod = GlobalVariables.usuario.Id;
            return _dao.Actualizar(bono);
        }


        public bool Eliminar(int id)
        {
            return _dao.Eliminar(id);
        }

        public bool EliminarTalonariosBeneficiario(int id)
        {

            return _dao.EliminarTalonarios(id);
        }

    }
}
