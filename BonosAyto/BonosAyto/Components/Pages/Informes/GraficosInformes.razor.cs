using Blazorise.Charts;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;
using Blazorise;

namespace BonosAyto.Components.Pages.Informes
{
    public partial class GraficosInformes
    {
        private BarChart<double> barChart;
        private BarChart<double> barChartComparativo;
        private List<string> nombresEstablecimientos = new();
        private string establecimientoSeleccionado;
        private string filtroSeleccionado = "Todos";
        private string tituloGrafico = String.Empty;

        // Se actualiza en cada cambio
        private async Task CambiarEstablecimiento(string nombre)
        {
            establecimientoSeleccionado = nombre;
            await ActualizarGrafico();
        }

        private async Task CambiarFiltro(string filtro)
        {
            filtroSeleccionado = filtro;
            await ActualizarGrafico();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var datos = await EstablecimientoService.ObtenerTodosLosNombresDeEstablecimientos();
                nombresEstablecimientos = datos;
                establecimientoSeleccionado = nombresEstablecimientos.FirstOrDefault();
                await ActualizarGrafico();
            }
        }

        private async Task ActualizarGrafico()
        {
            if (string.IsNullOrWhiteSpace(establecimientoSeleccionado))
                return;

            EstablecimientoDatosDTO item;

            if (filtroSeleccionado == "Todos")
            {
                item = await EstablecimientoService.ObtenerDatosPorEstablecimiento(establecimientoSeleccionado);
                tituloGrafico = "Bonos canjeados + importe por establecimiento";
            }
            else
            {
                item = await EstablecimientoService.ObtenerDatosUltimoTrimestrePorEstablecimiento(establecimientoSeleccionado);
                tituloGrafico = "Bonos canjeados + importe en el trimestre activo";
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
            await ActualizarGraficoComparativo();

        }

        private async Task ActualizarGraficoComparativo()
        {
            if (string.IsNullOrWhiteSpace(establecimientoSeleccionado))
                return;

            bool soloTrimestre = filtroSeleccionado == "Trimestre activo";
            var datos = await EstablecimientoService.ObtenerBonosEImportePorDiaSemana(establecimientoSeleccionado, soloTrimestre);

            if (barChartComparativo == null)
                return;

            await barChartComparativo.Clear();

            // Mapping de días (1 = domingo, 2 = lunes, ..., 7 = sábado)
            string[] diasES = new[] { "L", "M", "X", "J", "V", "S", "D" };  // Eje X
            var bonos = new List<double>();
            var importes = new List<double>();

            for (int i = 1; i <= 7; i++)  // De 1 (domingo) a 7 (sábado)
            {
                var dia = datos.ContainsKey(i) ? datos[i] : (0, 0.0);
                bonos.Add(dia.Item1);
                importes.Add(dia.Item2);
            }

            await barChartComparativo.AddLabels(diasES);

            await barChartComparativo.AddDataSet(new BarChartDataset<double>
            {
                Label = "Bonos Canjeados",
                Data = bonos,
                BackgroundColor = "rgba(54, 162, 235, 0.6)"
            });

            await barChartComparativo.AddDataSet(new BarChartDataset<double>
            {
                Label = "Importe Total",
                Data = importes,
                BackgroundColor = "rgba(255, 206, 86, 0.6)"
            });

            await barChartComparativo.Update();
        }

    }
}
