using Blazorise.Charts;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;
using Blazorise;

namespace BonosAyto.Components.Pages.Informes
{
    public partial class GraficosInformes
    {
        private List<string> nombresEstablecimientos = new();
        private string inputEstablecimiento;


        /****************************** ¡NUEVO! / CAMBIOS PARA AÑADIR COMPONENTES HIJO ************************************/
        private FiltrosInforme filtros = new();
        public class FiltrosInforme
        {
            public string EstablecimientoSeleccionado { get; set; }
            public string FiltroSeleccionado { get; set; } = "Total";
        }


        private async Task CambiarEstablecimiento(string nombre)
        {
            filtros = new FiltrosInforme
            {
                EstablecimientoSeleccionado = nombre,
                FiltroSeleccionado = filtros.FiltroSeleccionado
            };
            await InvokeAsync(StateHasChanged); // 'obliga a renderizarse de nuevo', por seguridad en la visualizacion de los cambios
        }

        private async Task CambiarFiltro(string filtro)
        {
            filtros = new FiltrosInforme
            {
                EstablecimientoSeleccionado = filtros.EstablecimientoSeleccionado,
                FiltroSeleccionado = filtro
            };
            await InvokeAsync(StateHasChanged); // 'obliga a renderizarse de nuevo', por seguridad en la visualizacion de los cambios
        }

        private void ValidarYSeleccionarEstablecimiento(ChangeEventArgs e)
        {
            inputEstablecimiento = e.Value?.ToString();

            if (string.IsNullOrWhiteSpace(inputEstablecimiento) || inputEstablecimiento == "Todos")
                CambiarEstablecimiento(null);
            else if (nombresEstablecimientos.Contains(inputEstablecimiento))
                CambiarEstablecimiento(inputEstablecimiento);
            else
                CambiarEstablecimiento(null);
        }

        protected override async Task OnInitializedAsync()
        {
            nombresEstablecimientos = await EstablecimientoService.ObtenerTodosLosNombresDeEstablecimientos();
            filtros.EstablecimientoSeleccionado = nombresEstablecimientos.FirstOrDefault();
        }

        /*******************************************************************************************************/



    }
}
