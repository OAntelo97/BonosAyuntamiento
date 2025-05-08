using BonosAytoService.DTOs;
using BonosAytoService.Models;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BonosAyto.Components.Pages.Usuarios
{
    public partial class DetalleUsuarios
    {
        [Parameter] public int Id { get; set; }
        [Parameter] public string? Modo { get; set; }

        [Inject]
        private UsuarioService UsuarioService { get; set; }
        [Inject]
        private EstablecimientoService EstablecimientoService { get; set; }

        private UsuarioDTO? usuario = new();

        [Inject]
        private IJSRuntime JS { get; set; }

        private bool EsModoLectura => Modo?.ToLower() != "editar";

        private string nombreEstablecimiento = string.Empty; //variable donde guardar el nombre del establecimiento con cierto Id para verlo en el form

        private IEnumerable<EstablecimientoDTO> establecimientos { get; set; }

        private string tituloError = "";
        private string MensajeErrorEliminar = "";
        protected override async Task OnInitializedAsync()
        {
            // Cargar el usuario desde la base de datos
            usuario = await UsuarioService.Consultar(Id);

            // Carga todos los establecimietos
            establecimientos = await EstablecimientoService.Listar();

            if (usuario?.IdEstablecimiento != null)
            {
                var establecimiento = await EstablecimientoService.Consultar(usuario.IdEstablecimiento.Value);
                nombreEstablecimiento = establecimiento?.Nombre ?? "Desconocido";
            }

        }

        private async Task GuardarCambios()
        {
            if (!EsModoLectura)
            {
                int res = await UsuarioService.Actualizar(usuario);
                // Comprobar si el nombre de usuario ya existe
                if (res <= 0)
                {
                    tituloError = "actualizar";
                    switch (res)
                    {
                        case -2:
                            MensajeErrorEliminar = "Ya existe un usuario con este nombre. Por favor, intriduzca otro nombre";
                            break;
                        case -3:
                            MensajeErrorEliminar = "Ya existe un usuario con este correo.";
                            break;
                        default:
                            MensajeErrorEliminar = "Se ha producido un error inesperado. Por favor, vuelva a intentarlo mas tarde";
                            break;
                    }
                    AbrirModal("EliminarError");
                    return;
                }


                Navigate.NavigateTo("/usuarios");
            }
        }

        /*

        private void VerDetalle()
        {
            Navigate.NavigateTo($"/usuarios/ver/{Id}");
        }
        private void Editar()
        {
            Navigate.NavigateTo($"/usuarios/editar/{Id}");
        }
        */
        private void VolverAtras()
        {
            Navigate.NavigateTo("/usuarios");
        }
        private async Task AbrirModal(string modalId)
        {
            await JS.InvokeVoidAsync("MODAL.AbrirModal", modalId);
        }

    }
}
