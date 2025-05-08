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


            /*Solo compruebo si es "Todos" o un establecimiento porque en el componente padre ya se comprueba que el datalist de establecimientos no pueda 
                estar vacio(ser null) ni espacios en blanco*/
            if (establecimiento == "Todos")
            {
                // Caso: "Todos"
                item = await EstablecimientoService.ObtenerDatosDeTodosLosEstablecimientos(soloTrimestre);
                tituloGrafico = soloTrimestre
                    ? "Bonos canjeados + Importe en el trimestre activo"
                    : "Bonos canjeados + Importe total";
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                // Caso: establecimiento seleccionado
                if (soloTrimestre)
                {
                    item = await EstablecimientoService.ObtenerDatosUltimoTrimestrePorEstablecimiento(establecimiento);
                    tituloGrafico = "Bonos canjeados + Importe en el trimestre activo";
                }
                else
                {
                    item = await EstablecimientoService.ObtenerDatosPorEstablecimiento(establecimiento);
                    tituloGrafico = "Bonos canjeados + Importe total";
                }
                await InvokeAsync(StateHasChanged);
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
