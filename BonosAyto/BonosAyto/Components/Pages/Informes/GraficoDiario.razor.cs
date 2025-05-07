using Blazorise.Charts;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;
using static BonosAyto.Components.Pages.Informes.GraficosInformes;

namespace BonosAyto.Components.Pages.Informes
{
    public partial class GraficoDiario
    {
        private BarChart<double> barChartDias;
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
                await ActualizarGraficoDias();
            }
        }


        private async Task ActualizarGraficoDias()
        {
            string establecimiento = Filtros.EstablecimientoSeleccionado;
            bool soloTrimestre = Filtros.FiltroSeleccionado != "Total";

           

            if (barChartDias != null)
            {
                await barChartDias.Clear();
            }

            Dictionary<int, (int Bonos, double Importe)> datos;

            if (string.IsNullOrWhiteSpace(establecimiento) || establecimiento == "Todos")
            {
                // Obtener datos de todos los establecimientos
                datos = await EstablecimientoService.ObtenerBonosEImportePorDiaSemanaTodos(soloTrimestre);
                if (soloTrimestre)
                {
                    tituloGrafico = "Bonos canjeados + Importe diario en el trimestre activo";
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    tituloGrafico = "Bonos canjeados + Importe diario total";
                    await InvokeAsync(StateHasChanged);
                }
            }
            else
            {
                // Obtener datos para el establecimiento seleccionado
                datos = await EstablecimientoService.ObtenerBonosEImportePorDiaSemana(establecimiento, soloTrimestre);
                if (soloTrimestre)
                {
                    tituloGrafico = "Bonos canjeados + Importe diario en el trimestre activo";
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    tituloGrafico = "Bonos canjeados + Importe diario total";
                    await InvokeAsync(StateHasChanged);
                }
            }


            // Orden correcto de días: lunes (2) a domingo (1) en SQL Server (cuando DATEFIRST = 1)
            string[] diasES = new[] { "L", "M", "X", "J", "V", "S", "D" };
            int[] indicesSQLServer = new[] { 1, 2, 3, 4, 5, 6, 7 }; // 1 = lunes, ..., 7 = domingo

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
                await barChartDias.Update();
                return;
            }

            await barChartDias.AddLabels(diasES);

            await barChartDias.AddDataSet(new BarChartDataset<double>
            {
                Label = "Bonos Canjeados",
                Data = bonos,
                BackgroundColor = "rgba(54, 162, 235, 0.6)"
            });

            await barChartDias.AddDataSet(new BarChartDataset<double>
            {
                Label = "Importe Total",
                Data = importes,
                BackgroundColor = "rgba(255, 206, 86, 0.6)"
            });

            await barChartDias.Update();
        }

    }
}
