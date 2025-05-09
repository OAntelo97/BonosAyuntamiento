using System.Text.Json;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using BonosAytoService.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace BonosAyto.Components.Pages.Login
{
    public partial class RecirdarPass
    {
        private UsuarioDTO usuario = new UsuarioDTO();
        public UsuarioService UsuarioService { get; set; } = new UsuarioService();
        private string mesageError = "";
        [Inject]
        private IJSRuntime JS { get; set; }
        private string codigo = "";
        private string usuairoCodigo = "";
        private string correoMostrar = "";
        private string textoRenviarCodigo = "¿No ha recibido el código? Pulse aquí para reenviarlo.";

        protected override void OnInitialized()
        {
            UsuarioService = new UsuarioService();
        }

        private async Task Validaciones()
        {
            mesageError = "";

            int id = await UsuarioService.comprobarNombreUsuario(usuario);
            if(codigo == "")
            {
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
                        mesageError = "";
                        string[] correioPartido = usuario.Email.Split('@');
                        correoMostrar = correioPartido[0].Substring(0, 2) + "......." + correioPartido[0].Substring(correioPartido[0].Length-2, 2) + "@" + correioPartido[1];
                        //Navigate.NavigateTo("/login");
                        EnviarCodigo();
                    }
                }
            }
            else
            {
                if (codigo != HashUtil.ObtenerHashSHA256(usuairoCodigo))
                {
                    mesageError = "El codigo introducido no conincide.";
                    textoRenviarCodigo = "Reenviar código";
                }
                else
                {
                    GlobalVariables.usuario = usuario;
                    Authenticate();
                }
            }
        }

        private void EnviarCodigo()
        {
            if(codigo != "")
            {
                textoRenviarCodigo = "El código ha sido reenviado correctamente.";
            }
            codigo = HashUtil.GenerarCodigoAlfanumerico(8);
            Console.WriteLine(codigo);
            codigo = HashUtil.ObtenerHashSHA256(codigo);
        }

        private async Task Authenticate()
        {
            LoginViewModel usuarioLogin = new LoginViewModel
            {
                Id = GlobalVariables.usuario.Id,
                Usuario = GlobalVariables.usuario.Usuario,
                Rol = GlobalVariables.usuario.Rol
            };
            var respuesta = await JS.InvokeAsync<string>("cookieHelper.authLogin", JsonSerializer.Serialize(usuarioLogin));

            Navigate.NavigateTo($"/login/CambiarContrasena/{usuario.Id}", forceLoad: true);
        }
    }
}
