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
        [Parameter] public string? Modo { get; set; } = "ver"; // "ver" ou "editar"

        [Inject]
        private UsuarioService UsuarioService { get; set; }
        [Inject]
        private EstablecimientoService EstablecimientoService { get; set; }

        private UsuarioDTO? usuario = new();

        [Inject]
        private IJSRuntime JS { get; set; }

        private bool EsModoLectura => Modo?.ToLower() != "editar";

        private string nombreEstablecimiento = string.Empty; //variable donde guardar el nombre del establecimiento con cierto Id para verlo en el form
        protected override void OnInitialized()
        {
            // Cargar el usuario desde la base de datos
            usuario = UsuarioService.Consultar(Id);

            if (usuario?.IdEstablecimiento != null)
            {
                var establecimiento = EstablecimientoService.Consultar(usuario.IdEstablecimiento.Value);
                nombreEstablecimiento = establecimiento?.Nombre ?? "Desconocido";
            }

        }

        private async Task GuardarCambios()
        {
            if (!EsModoLectura)
            {
                // Comprobar si el nombre de usuario ya existe
                if (UsuarioService.UsuarioExiste(usuario?.Usuario, usuario.Id))
                {
                    await JS.InvokeVoidAsync("alert", "Ese usuario ya está registrado. Escribe otro");
                    return; //rompe la ejecucion para que el id autoincremental no sigue contando pese a no tener lugar la insercion por un error
                }
                else if (UsuarioService.EmailExiste(usuario?.Email, usuario.Id))
                {
                    await JS.InvokeVoidAsync("alert", "Ese correo ya está registrado. Escribe otro");
                    return;
                }

                UsuarioService.Actualizar(usuario);
                Navigate.NavigateTo("/usuarios");
            }
        }

        private void VerDetalle(int id)
        {
            Navigate.NavigateTo($"/usuarios/ver/{id}");
        }
        private void Editar(int id)
        {
            Navigate.NavigateTo($"/usuarios/editar/{id}");
        }
        private void VolverAtras()
        {
            Navigate.NavigateTo("/usuarios");
        }
        
    }
}
