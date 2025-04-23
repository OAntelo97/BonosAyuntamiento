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

        private const string conn = "Server=DESKTOP-B5B66KI\\SQLEXPRESS;Database=pruebaHugo;Trusted_Connection=True; TrustServerCertificate=True;";

        public int Insertar(UsuarioDTO user)
        {
            var umap = _mapper.Map<Usuarios>(user);
            umap.UsuarioMod = GlobalVariables.usuario.Id;
            return _dao.Insertar(umap);
        }

        public UsuarioDTO? Consultar(int id)
        {
            var user = _dao.Consultar(id);
            return user == null ? null : _mapper.Map<UsuarioDTO>(user);
        }

        public int comprobarUsuario(UsuarioDTO user)
        {
            var umap = _mapper.Map<Usuarios>(user);
            return _dao.comprobarUsuario(umap);
        }


        public IEnumerable<UsuarioDTO> Listar()
        {
            var lista = _dao.Listar();
            return lista.Select(_mapper.Map<UsuarioDTO>);
        }

        public bool Actualizar(UsuarioDTO user)
        {
            var umap = _mapper.Map<Usuarios>(user);
            umap.UsuarioMod = GlobalVariables.usuario.Id;
            return _dao.Actualizar(umap);

        }
        public bool Eliminar(int id)
        {
            return _dao.Eliminar(id);
        }

        public bool UsuarioExiste(string user)
        {
            using var conection = new SqlConnection(conn);
            var sql = "SELECT * FROM dbo.Usuarios WHERE Usuario = @Usuario;";

            var usuario = conection.QueryFirstOrDefault<Usuarios>(sql, new { Usuario = user });

            return usuario != null;
        }
        public bool EmailExiste(string email)
        {
            using var conection = new SqlConnection(conn);
            var sql = "SELECT * FROM dbo.Usuarios WHERE Email = @Email;";

            var usuario = conection.QueryFirstOrDefault<Usuarios>(sql, new { Email = email });

            return usuario != null;
        }
    }
}
