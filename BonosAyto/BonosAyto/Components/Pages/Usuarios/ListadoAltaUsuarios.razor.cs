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


        private UsuarioDTO usuario = new UsuarioDTO();

        private IEnumerable<EstablecimientoDTO> establecimientos {  get; set; }

        private async Task GuardarUsuario()
        {
            var (res1, res2) = await UsuarioService.Insertar(usuario);
            // Comprobar si el nombre de usuario ya existe
            if (res2 == -2)
            {
                await JS.InvokeVoidAsync("alert", "Ese correo ya está registrado. Escribe otro");
                return; //rompe la ejecucion para no recargar la lista innecesariamnete
            }
            else if (res1 == -2)
            {
                await JS.InvokeVoidAsync("alert", "Ese usuario ya está registrado. Escribe otro");
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


    }
}




