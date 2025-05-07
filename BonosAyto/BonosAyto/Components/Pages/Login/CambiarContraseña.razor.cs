using Blazorise;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace BonosAyto.Components.Pages.Login
{
    public partial class CambiarContraseña
    {
        private UsuarioDTO usuario = new UsuarioDTO();
        private bool recordarme = false;
        public UsuarioService UsuarioService { get; set; } = new UsuarioService();
        EditContext editContext;
        ValidationMessageStore validationMessageStore;

        private (string contrasena, string repetido) nuevaContrasena  = ("", "");

        protected override void OnInitialized()
        {
            UsuarioService = new UsuarioService();

            editContext = new EditContext(usuario);
            validationMessageStore = new(editContext);

            editContext.OnValidationRequested += Validaciones;
        }

        private async void Validaciones(object sender, ValidationRequestedEventArgs e)
        {
            validationMessageStore.Clear();

            int id = await UsuarioService.comprobarUsuario(usuario);
            if (id == -1)
            {
                validationMessageStore.Add(() => usuario.Pass, "El nombre de usuario o la contraseña no son correctos");
                editContext.NotifyValidationStateChanged();
            }
            else if (nuevaContrasena.contrasena != nuevaContrasena.repetido)
            {
                validationMessageStore.Add(() => nuevaContrasena.repetido, "La repetida no es igual a la contraseña nueva");
                editContext.NotifyValidationStateChanged();
            }
            else
            {
                GlobalVariables.usuario = await UsuarioService.Consultar(id);
            }
        }

        private async Task Authenticate()
        {
            await UsuarioService.ActualizarContrasena(GlobalVariables.usuario);

            Navigate.NavigateTo("/login");
        }
    }
}
