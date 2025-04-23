using System.Diagnostics;
using Azure;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace BonosAyto.Components.Pages.Login
{
    public partial class Login
    {
        private UsuarioDTO usuario = new UsuarioDTO();
        public UsuarioService UsuarioService { get; set; }

        [Inject]
        IHttpContextAccessor httpContextAccessor { get; set; }

        EditContext editContext;

        ValidationMessageStore validationMessageStore;


        protected override void OnInitialized()
        {
            UsuarioService = new UsuarioService();

            editContext = new EditContext(usuario);
            validationMessageStore = new(editContext);

            // Add additional validation handler
            editContext.OnValidationRequested += IniciarSesion;

            var token = httpContextAccessor.HttpContext.Request.Cookies["UsId"];
            if (token!=null)
            {
                Navigate.NavigateTo("/");
            }

        }


        async void IniciarSesion(object sender, ValidationRequestedEventArgs e)
        {
            validationMessageStore.Clear();


            int id = UsuarioService.comprobarUsuario(usuario);

            if (id != -1) {
                GlobalVariables.usuario = UsuarioService.Consultar(id);
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddDays(30);
                httpContextAccessor.HttpContext.Response.Cookies.Append("UsId", usuario.Id.ToString(), options);
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
