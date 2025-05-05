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

        [Inject]
        private NavigationManager Navigation { get; set; }
        [Parameter]
        public bool edit { get; set; }
        EditContext bonoContext;
        ValidationMessageStore messageStore;

        private IEnumerable<BonoDTO> listaBonos;
        [Inject]
        private IJSRuntime JS { get; set; }




        private string tituloDetalleEstablecimiento { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            detalleE = await establecimientoService.Consultar(Id);

            canjeoasEstablecimiento = await establecimientoService.ConsulatarCanjeos(Id);
            titulo();
        }
        private void ModificarEstablecimiento()         //modificar establecimientos           
        {
            establecimientoService.Actualizar(detalleE);
            titulo();
        }

        private void titulo()
        {
            tituloDetalleEstablecimiento = $"Información de {detalleE.Nombre}";
        }

        private void AutoCaducidad()
        {
            bonoAsig.FechaCaducidad = bonoAsig.FechaInicio.AddMonths(3);
        }

        //botones accion
        private void VerDetalle(int id)
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
