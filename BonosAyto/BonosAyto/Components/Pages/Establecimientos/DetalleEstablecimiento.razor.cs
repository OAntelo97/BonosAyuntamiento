using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace BonosAyto.Components.Pages.Establecimientos
{
    public  partial class DetalleEstablecimiento
    {
        private EstablecimientoDTO detalleE = new EstablecimientoDTO();
        public AsignarBono bonoAsig;
        private EstablecimientoService establecimientoService = new EstablecimientoService();
        private BonoService bonoService = new BonoService();
        private IEnumerable<BenCanjBonEst> canjeoasEstablecimiento = [];
        [Parameter]
        public int Id { get; set; }
        [Parameter] 
        public string? Modo { get; set; } = "ver";

        [Inject]
        private NavigationManager Navigation { get; set; }
        EditContext bonoContext;
        ValidationMessageStore messageStore;

        private IEnumerable<BonoDTO> listaBonos;
        [Inject]
        private IJSRuntime JS { get; set; }

        private (int nCanjeos, float importeT) metricas = (0,0);

        private bool EsModoLectura => Modo?.ToLower() != "editar";

        private string tituloError = "";
        private string MensajeErrorEliminar = "";



        private string tituloDetalleEstablecimiento { get; set; }
        protected override async Task OnInitializedAsync()
        {
            metricas = await establecimientoService.ConsultarMetricas(Id);
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            detalleE = await establecimientoService.Consultar(Id);

            canjeoasEstablecimiento = await establecimientoService.ConsulatarCanjeos(Id);
            titulo();
        }
        private async Task ModificarEstablecimiento()         //modificar establecimientos           
        {
            int res = await establecimientoService.Actualizar(detalleE);
            if (res <= 0)
            {
                tituloError = "insertar";
                switch (res)
                {
                    case -2:
                        MensajeErrorEliminar = "Ya existe un usuario con este NIF";
                        break;
                    case -3:
                        MensajeErrorEliminar = "Ya existe un usuario con este correo. Por favor, intriduzca otro correo";
                        break;
                    default:
                        MensajeErrorEliminar = "Se ha producido un error inesperado. Por favor, vuelva a intentarlo mas tarde";
                        break;
                }
                AbrirModal("EliminarError");
                return;
            }
            VerDetalleEditar("ver", detalleE.Id);
        }

        private void titulo()
        {
            tituloDetalleEstablecimiento = $"Información de {detalleE.Nombre}";
        }

        private void VerDetalleEditar(string modo, int id)
        {
            Navigate.NavigateTo($"/establecimientos/{modo}/{id}");
        }

        private async Task AbrirModal(string modalId)
        {
            await JS.InvokeVoidAsync("MODAL.AbrirModal", modalId);
        }


        //botones accion
        private void VerDetalleCanjeo(int id)
        {
            Navigate.NavigateTo($"/bonos/detalletalonario/{id}?edit=false");
        }
        private void Modificar(int id)
        {
            Navigate.NavigateTo($"/bonos/detalletalonario/{id}?edit=true");
        }
        

        private async Task Borrar(int id)
        {
            //bool confirmed = await JS.InvokeAsync<bool>("confirm", $"¿Está seguro de que desea borrar este talonario?");
            //if (confirmed)
            //{
            //    bonoService.Eliminar(id);
            //    listaBonos = bonoService.Listar(Id);
            //}
        }
    }
}
