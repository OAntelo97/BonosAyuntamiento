using Blazorise.Charts;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;

namespace BonosAyto.Components.Pages.Informes
{
    public partial class GraficosInformes
    {

        private BarChart<double> barChart;
        private List<EstablecimientoDatosDTO> datos = new();
        private List<string> nombresEstablecimientos = new();
        private string establecimientoSeleccionado;

        private bool primerRender = true;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                datos = await EstablecimientoService.ObtenerDatosPorEstablecimiento();  // Cambié la llamada para que sea await
                nombresEstablecimientos = datos.Select(d => d.NombreEstablecimiento).ToList();
                await SeleccionarEstablecimiento(nombresEstablecimientos.FirstOrDefault());
                primerRender = false;
            }
        }

        private async Task SeleccionarEstablecimiento(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return;

            establecimientoSeleccionado = nombre;

            if (datos == null || !datos.Any()) return;

            var item = datos.FirstOrDefault(d => d?.NombreEstablecimiento == nombre);

            if (item == null)
            {
                if (barChart != null)
                {
                    await barChart.Clear();
                    await barChart.Update();
                }
                return;
            }

            if (barChart == null) return;

            // Asegúrate de limpiar antes de agregar nuevos datasets
            await barChart.Clear();

            // Configurar etiquetas
            await barChart.AddLabels(new List<string> { item.NombreEstablecimiento });

            // Agregar el primer dataset: Bonos Canjeados
            await barChart.AddDataSet(new BarChartDataset<double>
            {
                Label = "Bonos Canjeados",
                Data = new List<double> { item.BonosCanjeados },
                BackgroundColor = "rgba(54, 162, 235, 0.6)"
            });

            // Agregar el segundo dataset: Importe Total
            await barChart.AddDataSet(new BarChartDataset<double>
            {
                Label = "Importe Total",
                Data = new List<double> { (double)item.ImporteTotal },
                BackgroundColor = "rgba(255, 99, 132, 0.6)"
            });

            await barChart.Update(); // Actualiza el gráfico
        }
    }


    }

