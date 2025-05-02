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
        private BarChart<double> barChartMensual;
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
            EstablecimientoDatosDTO item;

            if (string.IsNullOrWhiteSpace(establecimientoSeleccionado) || establecimientoSeleccionado == "Todos")
            {
                // Si se seleccionó "Todos", obtener los datos de todos los establecimientos
                item = await EstablecimientoService.ObtenerDatosDeTodosLosEstablecimientos();
                tituloGrafico = "Bonos canjeados + importe de todos los establecimientos";
            }
            else
            {
                // Si se seleccionó un establecimiento específico
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
            }

            if (barChart == null)
                return;

            await barChart.Clear();

            if (item == null)
            {
                await barChart.Update();
                return;
            }

            // Aquí agregamos los datos al gráfico como lo hacíamos antes
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
            await ActualizarGraficoMensual();
        }




        private async Task ActualizarGraficoComparativo()
        {
            if (string.IsNullOrWhiteSpace(establecimientoSeleccionado) && filtroSeleccionado != "Todos")
                return;

            if (barChartComparativo != null)
            {
                // Limpiamos el gráfico
                await barChartComparativo.Clear();
            }

            bool soloTrimestre = filtroSeleccionado == "Trimestre activo";

            var datos = new Dictionary<int, (int, double)>();

            if (filtroSeleccionado == "Todos")
            {
                // Obtener datos de todos los establecimientos
                datos = await EstablecimientoService.ObtenerBonosEImportePorDiaSemanaTodos(soloTrimestre);
            }
            else
            {
                // Obtener datos para un solo establecimiento
                datos = await EstablecimientoService.ObtenerBonosEImportePorDiaSemana(establecimientoSeleccionado, soloTrimestre);
            }

            string[] diasES = new[] { "L", "M", "X", "J", "V", "S", "D" };
            int[] indicesSQLServer = new[] { 2, 3, 4, 5, 6, 7, 1 };

            var bonos = new List<double>();
            var importes = new List<double>();

            foreach (var i in indicesSQLServer)
            {
                var dia = datos.ContainsKey(i) ? datos[i] : (0, 0.0);
                bonos.Add(dia.Item1);
                importes.Add(dia.Item2);
            }

            bool tieneDatos = bonos.Any(b => b > 0) || importes.Any(i => i > 0);

            if (!tieneDatos)
            {
                await barChartComparativo.Update();
                return;
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





        private async Task ActualizarGraficoMensual()
        {
            if (string.IsNullOrWhiteSpace(establecimientoSeleccionado) && filtroSeleccionado != "Todos")
                return;

            if (barChartMensual != null)
                await barChartMensual.Clear();

            bool soloTrimestre = filtroSeleccionado == "Trimestre activo";

            var datos = new Dictionary<int, (int, double)>();

            if (filtroSeleccionado == "Todos")
            {
                // Obtener datos de todos los establecimientos
                datos = await EstablecimientoService.ObtenerBonosEImportePorMesTodos(soloTrimestre);
            }
            else
            {
                // Obtener datos para un solo establecimiento
                datos = await EstablecimientoService.ObtenerBonosEImportePorMes(establecimientoSeleccionado, soloTrimestre);
            }

            var mesesES = new[] { "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };

            var bonos = new List<double>();
            var importes = new List<double>();

            for (int i = 1; i <= 12; i++)
            {
                var mes = datos.ContainsKey(i) ? datos[i] : (0, 0.0);
                bonos.Add(mes.Item1);
                importes.Add(mes.Item2);
            }

            bool tieneDatos = bonos.Any(b => b > 0) || importes.Any(i => i > 0);
            if (!tieneDatos)
            {
                await barChartMensual.Update();
                return;
            }

            await barChartMensual.AddLabels(mesesES);

            await barChartMensual.AddDataSet(new BarChartDataset<double>
            {
                Label = "Bonos Canjeados",
                Data = bonos,
                BackgroundColor = "rgba(75, 192, 192, 0.6)"
            });

            await barChartMensual.AddDataSet(new BarChartDataset<double>
            {
                Label = "Importe Total",
                Data = importes,
                BackgroundColor = "rgba(153, 102, 255, 0.6)"
            });

            await barChartMensual.Update();
        }



    }
}
