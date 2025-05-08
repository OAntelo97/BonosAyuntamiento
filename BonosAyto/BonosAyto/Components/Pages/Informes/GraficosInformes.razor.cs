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
        private string mensajeAlert = string.Empty;
        private bool soloTrimestre = false; 


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

        private async Task ValidarYSeleccionarEstablecimiento(ChangeEventArgs e)
        {
            inputEstablecimiento = e.Value?.ToString();

            InvokeAsync(StateHasChanged);   

            //comprobacion de que el input no está con espacios en blanco ni vacio
            if (string.IsNullOrWhiteSpace(inputEstablecimiento))
            {
                mensajeAlert = "Debes introducir un establecimiento o \"Todos\".";
                inputEstablecimiento = string.Empty;
            }
            else
            {
                var datos = await EstablecimientoService.ObtenerDatosPorEstablecimiento(inputEstablecimiento);

                //si se escribe un establecimiento (sin ser Todos), se comprueba si tiene datos, si no tiene se muestra un alert
                if (datos == null && inputEstablecimiento != "Todos")
                {
                    mensajeAlert = $"No se encontraron datos para \"{inputEstablecimiento}\".";
                    inputEstablecimiento = string.Empty;
                }
                else
                {
                    mensajeAlert = string.Empty;

                    if (inputEstablecimiento == "Todos")
                        CambiarEstablecimiento("Todos");
                    else if (nombresEstablecimientos.Contains(inputEstablecimiento))
                        CambiarEstablecimiento(inputEstablecimiento);
                    else
                        CambiarEstablecimiento(null);

                }
            }
        }


        protected override async Task OnInitializedAsync()
        {
            nombresEstablecimientos = await EstablecimientoService.ObtenerTodosLosNombresDeEstablecimientos();
            filtros.EstablecimientoSeleccionado = nombresEstablecimientos.FirstOrDefault();
        }

        /*******************************************************************************************************/



    }
}
