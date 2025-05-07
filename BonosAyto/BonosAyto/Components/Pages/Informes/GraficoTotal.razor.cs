using Blazorise.Charts;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;
using static BonosAyto.Components.Pages.Informes.GraficosInformes;

namespace BonosAyto.Components.Pages.Informes
{
    public partial class GraficoTotal
    {
        private BarChart<double> barChart;
        private string establecimientoSeleccionado;
        private string tituloGrafico = String.Empty;
        private List<string> nombresEstablecimientos = new();
        private bool _debeActualizarGrafico = false;
        [CascadingParameter] public FiltrosInforme Filtros { get; set; }


        protected override Task OnParametersSetAsync()
        {
            _debeActualizarGrafico = true;
            return Task.CompletedTask;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var datos = await EstablecimientoService.ObtenerTodosLosNombresDeEstablecimientos();
                nombresEstablecimientos = datos;
                establecimientoSeleccionado = nombresEstablecimientos.FirstOrDefault();
                _debeActualizarGrafico = true; // forzar actualización tras primer render
            }

            if (_debeActualizarGrafico)
            {
                _debeActualizarGrafico = false;
                await ActualizarGrafico();
            }
        }


        private async Task ActualizarGrafico()
        {
            string establecimiento = Filtros.EstablecimientoSeleccionado;
            bool soloTrimestre = Filtros.FiltroSeleccionado != "Total";
            EstablecimientoDatosDTO item;

            if (string.IsNullOrWhiteSpace(establecimiento) || establecimiento == "Todos")
            {
                item = await EstablecimientoService.ObtenerDatosDeTodosLosEstablecimientos(soloTrimestre);
                if (soloTrimestre)
                {
                    tituloGrafico = "Bonos canjeados + Importe en el trimestre activo";
                    await InvokeAsync(StateHasChanged); // 'obliga a renderizarse de nuevo', para que se cambien los titulos
                }
                else
                {
                    tituloGrafico = "Bonos canjeados + Importe total";
                    await InvokeAsync(StateHasChanged); 
                }
            }
            else
            {
                if (soloTrimestre)
                {
                    item = await EstablecimientoService.ObtenerDatosUltimoTrimestrePorEstablecimiento(establecimiento);
                    tituloGrafico = "Bonos canjeados + Importe en el trimestre activo";
                    await InvokeAsync(StateHasChanged); 
                }
                else
                {
                    item = await EstablecimientoService.ObtenerDatosPorEstablecimiento(establecimiento);
                    tituloGrafico = "Bonos canjeados + Importe total";
                    await InvokeAsync(StateHasChanged);
                }
            }


            if (barChart == null)
                return;

            await barChart.Clear();

            if (item == null)
            {
                await barChart.Update();
                return;
            }

            await barChart.AddLabels(new List<string> { item.NombreEstablecimiento });

            await barChart.AddDataSet(new BarChartDataset<double>
            {
                Label = "Bonos Canjeados",
                Data = new List<double> { item.BonosCanjeados },
                BackgroundColor = "rgba(54, 162, 235, 0.6)"
            });

            await barChart.AddDataSet(new BarChartDataset<double>
            {
                Label = "Importe Total",
                Data = new List<double> { (double)item.ImporteTotal },
                BackgroundColor = "rgba(255, 99, 132, 0.6)"
            });

            await barChart.Update();


        }

    }
}
