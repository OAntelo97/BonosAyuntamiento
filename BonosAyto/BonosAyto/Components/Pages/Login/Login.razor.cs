using System.Diagnostics;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components.Forms;

namespace BonosAyto.Components.Pages.Login
{
    public partial class Login
    {
        private UsuarioDTO usuario = new UsuarioDTO();
        public UsuarioService UsuarioService { get; set; }

        EditContext editContext;

        ValidationMessageStore validationMessageStore;


        protected override void OnInitialized()
        {
            UsuarioService = new UsuarioService();

            editContext = new EditContext(usuario);
            validationMessageStore = new(editContext);

            // Add additional validation handler
            editContext.OnValidationRequested += IniciarSesion;
            
        }


        async void IniciarSesion(object sender, ValidationRequestedEventArgs e)
        {
            validationMessageStore.Clear();


            int id = await UsuarioService.comprobarUsuario(usuario);

            if (id != -1) {
                GlobalVariables.usuario = await UsuarioService.Consultar(id);
                Navigate.NavigateTo("/");
            }
            else
            {
                validationMessageStore.Add(() => usuario.Pass, "El nombre de usuario o la contraseña no son correctos");
                editContext.NotifyValidationStateChanged();
            }
        }

        void prueba()
        {

        }
    }
}
