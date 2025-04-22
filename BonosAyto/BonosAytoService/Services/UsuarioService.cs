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
    public partial class UsuarioService
    {
        private readonly UsuarioDAO _dao;
        private readonly IMapper _mapper;

        public UsuarioService()
        {
            _dao = new UsuarioDAO();
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Usuarios, UsuarioDTO>();
                cfg.CreateMap<UsuarioDTO, Usuarios>();
            });
            _mapper = config.CreateMapper();

        }

        private const string conn = "Server=DESKTOP-B5B66KI\\SQLEXPRESS;Database=pruebaHugo;Trusted_Connection=True; TrustServerCertificate=True;";

        public void Insertar(UsuarioDTO user)
        {
            var umap = _mapper.Map<Usuarios>(user);
            _dao.Insertar(umap);
        }

        public UsuarioDTO? Consultar(int id)
        {
            var user = _dao.Consultar(id);
            return user == null ? null : _mapper.Map<UsuarioDTO>(user);
        }

        public IEnumerable<UsuarioDTO> Listar()
        {
            var lista = _dao.Listar();
            return lista.Select(_mapper.Map<UsuarioDTO>);
        }

        public bool Actualizar(UsuarioDTO user)
        {
            var umap = _mapper.Map<Usuarios>(user);
            return _dao.Actualizar(umap);

        }
        public bool Eliminar(int id)
        {
            return _dao.Eliminar(id);
        }
    }
}
