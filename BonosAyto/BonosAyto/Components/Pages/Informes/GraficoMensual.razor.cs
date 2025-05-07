using Blazorise.Charts;
using BonosAytoService.DTOs;
using Microsoft.AspNetCore.Components;
using static BonosAyto.Components.Pages.Informes.GraficosInformes;

namespace BonosAyto.Components.Pages.Informes
{
    public partial class GraficoMensual
    {
        private BarChart<double> barChartMensual;
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
                await ActualizarGraficoMensual();
            }
        }


        private async Task ActualizarGraficoMensual()
        {

            string establecimiento = Filtros.EstablecimientoSeleccionado;
            bool soloTrimestre = Filtros.FiltroSeleccionado != "Total";
            EstablecimientoDatosDTO item;


            if (barChartMensual != null)
            {
                await barChartMensual.Clear();
            }


            Dictionary<int, (int Bonos, double Importe)> datos;

            if (string.IsNullOrWhiteSpace(establecimiento) || establecimiento == "Todos")
            {
                datos = await EstablecimientoService.ObtenerBonosEImportePorMesTodos(soloTrimestre);
                if (soloTrimestre)
                {
                    tituloGrafico = "Bonos canjeados + Importe mensual en el trimestre activo";
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    tituloGrafico = "Bonos canjeados + Importe mensual total";
                    await InvokeAsync(StateHasChanged);
                }
            }
            else
            {
                datos = await EstablecimientoService.ObtenerBonosEImportePorMes(establecimiento, soloTrimestre);
                if (soloTrimestre)
                {
                    tituloGrafico = "Bonos canjeados + Importe mensual en el trimestre activo";
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    tituloGrafico = "Bonos canjeados + Importe mensual total";
                    await InvokeAsync(StateHasChanged);
                }
            }

            string[] nombresMeses = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            var bonos = new List<double>();
            var importes = new List<double>();


            foreach (var mes in datos)
            {
                // Agregar los datos a las listas correspondientes
                bonos.Add(mes.Value.Bonos);
                importes.Add(mes.Value.Importe);
            }



            await barChartMensual.AddLabels(nombresMeses);

            await barChartMensual.AddDataSet(new BarChartDataset<double>
            {
                Label = "Bonos Canjeados",
                Data = bonos,
                BackgroundColor = "rgba(153, 102, 255, 0.6)"
            });

            await barChartMensual.AddDataSet(new BarChartDataset<double>
            {
                Label = "Importe Total",
                Data = importes,
                BackgroundColor = "rgba(255, 159, 64, 0.6)"
            });

            // Verificar el estado final de los datos antes de actualizar el gráfico
            Console.WriteLine("Datos antes de actualizar el gráfico:");
            Console.WriteLine($"Bonos: {string.Join(", ", bonos)}");
            Console.WriteLine($"Importes: {string.Join(", ", importes)}");

            if (bonos.Count != 12 || importes.Count != 12)
            {
                Console.WriteLine("Error: Los datos no tienen 12 elementos.");
                return;
            }


            await barChartMensual.Update();
        }
    }
}
