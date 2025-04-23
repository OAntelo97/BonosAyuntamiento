using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;

namespace BonosAyto.Components.Pages.Usuarios
{
    public partial class DetalleUsuarios
    {
        [Inject]
        private UsuarioService UsuarioService { get; set; }


        [Parameter]
        public int Id { get; set; }

        private UsuarioDTO? usuario;

        protected override void OnInitialized()
        {
            usuario = UsuarioService.Consultar(Id);
        }

        private void alumnosMatriculadosMaterias()
        {
            Navigate.NavigateTo($"/usuarios/detalle/{Id}/matriculas");
        }

        private void volverAListado()
        {
            Navigate.NavigateTo("/usuarios");
        }
    }
}
