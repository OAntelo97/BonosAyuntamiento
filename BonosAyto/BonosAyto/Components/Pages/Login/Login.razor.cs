using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Azure;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace BonosAyto.Components.Pages.Login
{
    public partial class Login
    {
        private UsuarioDTO usuario = new UsuarioDTO();
        private bool recordarme = false;
        public UsuarioService UsuarioService { get; set; }
        [Inject] private IJSRuntime JS { get; set; }
        EditContext editContext;
        ValidationMessageStore validationMessageStore;

        protected override async void OnInitialized()
        {
            UsuarioService = new UsuarioService();

            editContext = new EditContext(usuario);
            validationMessageStore = new(editContext);
            //HttpContext = httpContextAccessor.HttpContext;

            editContext.OnValidationRequested += (object sender, ValidationRequestedEventArgs e) => { validationMessageStore.Clear(); };
            //var token = httpContextAccessor.HttpContext.Request.Cookies["UsId"];
            //if (token != null)
            //{
            //    Navigate.NavigateTo("/");
            //}

            //string cookie = await GetCookie("UsId");
            //if (cookie != null)
            //{
            //    GlobalVariables.usuario = UsuarioService.Consultar(Int32.Parse(cookie));
            //    await SetCookie("UsId", GlobalVariables.usuario.Id.ToString(), 30);
            //    Navigate.NavigateTo("/home");
            //}

            //var user = new UsuarioDTO
            //{
            //    Usuario = "Pablo",
            //    Pass = "1234",
            //    Rol = 'G',
            //    Email = "pablo.c@galsoft.es"
            //};

            //UsuarioService.Insertar(user);
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

        //private async Task Authenticate()
        //{
        //    var claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, usuario.Usuario)
        //        };

        //    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //    var principal = new ClaimsPrincipal(identity);
        //    await HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        //    GlobalVariables.usuario = UsuarioService.Consultar(usuario.Id);
        //    Navigate.NavigateTo("/home");
            

        //}

        //private async Task Authenticate2()
        //{
        //    await SetCookie("UsId", usuario.Id.ToString(), 30);
        //    await AuthService.Authenticate(usuario);
        //}

        private async Task Authenticate()
        {
            LoginViewModel usuarioLogin = new LoginViewModel
            {
                Id  = GlobalVariables.usuario.Id,
                Usuario = GlobalVariables.usuario.Usuario,
                Rol = GlobalVariables.usuario.Rol
            };
            //var client = ClientFactory.CreateClient("LoginApi");
            //var response = await client.PostAsJsonAsync("api/auth/login", usuarioLogin);
            var respuesta = await JS.InvokeAsync<string>("cookieHelper.authLogin", JsonSerializer.Serialize(usuarioLogin));

            Navigate.NavigateTo("/home", forceLoad: true);
        }
        

        private async Task Validaciones()
        {
            validationMessageStore.Clear();

            int id = await UsuarioService.comprobarUsuario(usuario);
            if (id == -1)
            {
                validationMessageStore.Add(() => usuario.Pass, "El nombre de usuario o la contraseña no son correctos");
                editContext.NotifyValidationStateChanged();
            }
            else
            {
                GlobalVariables.usuario = await UsuarioService.Consultar(id);
                Authenticate();
            }
        }

        async Task<string?> GetCookie(string name)
        {
            return await JS.InvokeAsync<string>("cookieHelper.getCookie", name);
        }

        async Task SetCookie(string name, string value, int days)
        {
            await JS.InvokeVoidAsync("cookieHelper.setCookie", name, value, days);
        }
    }
}
