using Blazorise;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace BonosAyto.Components.Pages.Login
{
    public partial class CambiarContraseña
    {
        private UsuarioDTO usuario = new UsuarioDTO();
        public UsuarioService UsuarioService { get; set; } = new UsuarioService();
        private string mesageError = "";

        private (string contrasena, string repetido) nuevaContrasena  = ("", "");

        protected override void OnInitialized()
        {
            UsuarioService = new UsuarioService();
        }

        private async Task Validaciones()
        {
            mesageError = "";

            int id = await UsuarioService.comprobarUsuario(usuario);
            if (id == -1)
            {
                mesageError = "El nombre de usuario o la contraseña no son correctos";
            }
            else if (nuevaContrasena.contrasena != nuevaContrasena.repetido)
            {
                mesageError = "Las contraseñas no coninciden";
            }
            else
            {
                Authenticate(await UsuarioService.Consultar(id));
            }
        }

        private async Task Authenticate(UsuarioDTO usu)
        {
            usu.Pass = nuevaContrasena.contrasena;
            await UsuarioService.ActualizarContrasena(usu);

            Navigate.NavigateTo("/login");
        }
    }
}
