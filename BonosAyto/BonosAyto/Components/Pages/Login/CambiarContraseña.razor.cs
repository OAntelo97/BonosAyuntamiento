using System.Security.Claims;
using Blazorise;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BonosAyto.Components.Pages.Login
{
    public partial class CambiarContraseña
    {
        private UsuarioDTO usuario = new UsuarioDTO();
        public UsuarioService UsuarioService { get; set; } = new UsuarioService();
        private string mesageError = "";

        private (string contrasena, string repetido) nuevaContrasena  = ("", "");
        private bool autorizado = false;
        [Parameter]
        public int? Id { get; set; }


        protected override async Task OnInitializedAsync()
        {
            if (Id != null) {
                var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (user.Identity is { IsAuthenticated: true })
                {
                    string userId = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                    if (userId == Id.ToString())
                    {
                        usuario = await UsuarioService.Consultar(Id ?? 0);

                    }
                    else
                    {
                        Navigate.NavigateTo("/login/CambiarContrasena");
                    }
                }
            }
        }

        private async Task Validaciones()
        {
            mesageError = "";

            int id = await UsuarioService.comprobarUsuario(usuario);
            if (id == -1 && Id == null)
            {
                mesageError = "El nombre de usuario o la contraseña no son correctos";
            }
            else if (nuevaContrasena.contrasena != nuevaContrasena.repetido)
            {
                mesageError = "Las contraseñas no coninciden";
            }
            else
            {
                Authenticate(Id == null ? await UsuarioService.Consultar(id): usuario);
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
