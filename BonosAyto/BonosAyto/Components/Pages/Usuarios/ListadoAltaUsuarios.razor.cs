using Microsoft.AspNetCore.Components;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.JSInterop;

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

        private async Task GuardarUsuario()
        {
            // Comprobar si el nombre de usuario ya existe
            if (UsuarioService.UsuarioExiste(usuario.Usuario))
            {
                await JS.InvokeVoidAsync("alert", "Ese usuario ya está registrado. Escribe otro");
                return; //rompe la ejecucion para que el id autoincremental no sigue contando pese a no tener lugar la insercion por un error
            }else if(UsuarioService.EmailExiste(usuario.Email)){
                await JS.InvokeVoidAsync("alert", "Ese correo ya está registrado. Escribe otro");
                return;
            }

            // Si no existe, insertar
            int nuevoId = UsuarioService.Insertar(usuario);
            Navigate.NavigateTo("/usuarios");
        }


        private string busqueda = string.Empty;
        private IEnumerable<UsuarioDTO> FiltrarUsuarios()
        {
            //Usamos .AsEnumerable() para asegurarnos de que estamos trabajando con una secuencia que puede ser filtrada usando LINQ (Language Integrated Query).
            var usuariosFiltrados = UsuarioService.Listar().AsEnumerable();


            /*Para filtrar todo usamos el método .Where(), que es parte de LINQ y filtra los elementos de la secuencia.
             Funciona como el where de sql*/

            // Filtrar por nombre
            if (!string.IsNullOrEmpty(busqueda))
            {
                usuariosFiltrados = usuariosFiltrados.Where(u => u.Usuario.Contains(busqueda, StringComparison.OrdinalIgnoreCase));
            }
            
            return usuariosFiltrados;
        }





        /******************LISTADO*****************/
        private IEnumerable<UsuarioDTO> usuarios;

        protected override void OnInitialized()
        {
            // Obtener todas las inscripciones usando el servicio y un ienumerable
            usuarios = UsuarioService.Listar();
        }


        private void VerDetalle(int id)
        {
            Navigate.NavigateTo($"/usuarios/detalle/{id}");
        }
        private void Editar(int id)
        {
            Navigate.NavigateTo($"/usuarios/editar/{id}");
        }
        private void Eliminar(int id)
        {
            var eliminado = UsuarioService.Eliminar(id);
            // Actualizar la lista después de eliminar la inscripción
            usuarios = UsuarioService.Listar();
        }

        private void DarDeAlta()
        {
            Navigate.NavigateTo("/materias/altas");
        }
    }
}




