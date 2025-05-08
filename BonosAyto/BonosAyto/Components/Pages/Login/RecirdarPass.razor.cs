using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BonosAyto.Components.Pages.Login
{
    public partial class RecirdarPass
    {
        private UsuarioDTO usuario = new UsuarioDTO();
        public UsuarioService UsuarioService { get; set; } = new UsuarioService();
        private string mesageError = "";
        [Inject]
        private EmailService EmailService { get; set; }

        protected override void OnInitialized()
        {
            UsuarioService = new UsuarioService();
        }

        private async Task Validaciones()
        {
            mesageError = "";

            int id = await UsuarioService.comprobarNombreUsuario(usuario);
            if (id == -1)
            {
                mesageError = "No se a encontrado un usuario con este nombre";
            }
            else
            {
                usuario = await UsuarioService.Consultar(id);
                if (usuario.Email == null)
                {
                    mesageError = "Este usuario no tiene un correo electrónico asociado para recuperar la contraseña.";
                }
                else
                {
                    //await EmailService.SendEmailAsync(usuario.Email, "peuba", "prueba  envio de correo");
                    Navigate.NavigateTo("/login");
                }
            }
        }

        private async Task Authenticate()
        {
            //await UsuarioService.ActualizarContrasena(GlobalVariables.usuario);

            Navigate.NavigateTo("/login");
        }
    }
}
