using AutoMapper;
using BonosAytoService.DAOs;
using BonosAytoService.DTOs;
using BonosAytoService.Model;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.Services
{
    public class UsuarioService
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

        public async Task<int> Insertar(UsuarioDTO user)
        {
            var umap = _mapper.Map<Usuarios>(user);
            umap.FechaMod = DateTime.Now;
            umap.UsuarioMod = GlobalVariables.usuario.Id;
            return await _dao.Insertar(umap);
        }

        public async Task<UsuarioDTO?> Consultar(int id)
        {
            var user = await _dao.Consultar(id);
            return user == null ? null : _mapper.Map<UsuarioDTO>(user);
        }

        public async Task<IEnumerable<UsuarioDTO>> ConsultarPorEstablecimiento(int idEstablecimiento)
        {
            var lista = await _dao.ConsultarPorEstablecimiento(idEstablecimiento);
            return lista.Select(_mapper.Map<UsuarioDTO>);
        }

        public async Task<int> comprobarUsuario(UsuarioDTO user)
        {
            var umap = _mapper.Map<Usuarios>(user);
            return await _dao.comprobarUsuario(umap);
        }
        
        public async Task<int> comprobarNombreUsuario(UsuarioDTO user)
        {
            var umap = _mapper.Map<Usuarios>(user);
            return await _dao.comprobarNombreUsuario(umap);
        }


        public async Task<IEnumerable<UsuarioDTO>> Listar()
        {
            var lista = await _dao.Listar();
            return lista.Select(_mapper.Map<UsuarioDTO>);
        }

        public async Task<int> Actualizar(UsuarioDTO user)
        {
            var umap = _mapper.Map<Usuarios>(user);
            umap.FechaMod = DateTime.Now;
            umap.UsuarioMod = GlobalVariables.usuario.Id;
            return await _dao.Actualizar(umap);

        }

        public async Task<bool> ActualizarContrasena(UsuarioDTO user)
        {
            var umap = _mapper.Map<Usuarios>(user);
            return await _dao.ActualizarContrasena(umap);
        }

        public async Task<bool> Eliminar(int id)
        {
            return await _dao.Eliminar(id);
        }
    }
}
