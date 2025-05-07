using Microsoft.AspNetCore.Components;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BonosAyto.Components.Pages.Usuarios
{
    public partial class ListadoAltaUsuarios
    {
        /******************FORM DE ALTAS*****************/
        [Inject]
        private UsuarioService UsuarioService { get; set; }
        [Inject]
        private EstablecimientoService EstablecimientoService { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }

        private bool cargado = false;

        private UsuarioDTO usuario = new UsuarioDTO();

        private int IdElimunar = 0;

        private IEnumerable<EstablecimientoDTO> establecimientos {  get; set; }

        private string tituloError = "";
        private string MensajeErrorEliminar = "";

        private async Task GuardarUsuario()
        {
            int res = await UsuarioService.Insertar(usuario);
            if (res <= 0)
            {
                tituloError = "insertar";
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

            usuarios = await UsuarioService.Listar(); //asi se recarga la lista despues de insertar para que los nuevos registros se muestren en la tabla tmb, y no solo cuando se haga f5

            Navigate.NavigateTo("/usuarios");
        }






        /******************LISTADO*****************/
        private IEnumerable<UsuarioDTO> usuarios = [];
        

        protected override async Task OnInitializedAsync()
        {
            // Obtener todas las inscripciones usando el servicio y un ienumerable
            usuarios = await UsuarioService.Listar();

            // Obtener todos los establecimietos
            establecimientos = await EstablecimientoService.Listar();
            cargado = true;
        }


        private void VerDetalle(int id)
        {
            Navigate.NavigateTo($"/usuarios/ver/{id}");
        }
        private void Editar(int id)
        {
            Navigate.NavigateTo($"/usuarios/editar/{id}");
        }
        private async Task Eliminar(int id)
        {
            var eliminado = await UsuarioService.Eliminar(id);
            // Actualizar la lista después de eliminar la inscripción
            usuarios = await UsuarioService.Listar();
        }

        private async Task AbrirModal(string modalId)
        {
            await JS.InvokeVoidAsync("MODAL.AbrirModal", modalId);
        }


    }
}




