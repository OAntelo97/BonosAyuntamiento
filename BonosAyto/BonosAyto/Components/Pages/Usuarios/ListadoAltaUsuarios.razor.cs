using Microsoft.AspNetCore.Components;
using BonosAytoService.DTOs;

namespace BonosAyto.Components.Pages.Usuarios
{
    public partial class ListadoAltaUsuarios
    {
        [Inject]
        private UsuarioService UsuarioService { get; set; }

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




