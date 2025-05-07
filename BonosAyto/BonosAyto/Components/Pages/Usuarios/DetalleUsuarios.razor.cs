using BonosAytoService.DTOs;
using BonosAytoService.Model;
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
                var (res1, res2) = await UsuarioService.Actualizar(usuario);
                // Comprobar si el nombre de usuario ya existe
                if (res2 == -2)
                {
                    await JS.InvokeVoidAsync("alert", "Ese correo ya está registrado. Escribe otro");
                    return;
                }else if (res1 == -2)
                {
                    await JS.InvokeVoidAsync("alert", "Ese usuario ya está registrado. Escribe otro");
                    return; //rompe la ejecucion para que el id autoincremental no sigue contando pese a no tener lugar la insercion por un error
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
        
    }
}
