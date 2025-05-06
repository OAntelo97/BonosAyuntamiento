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
        private string filtroSeleccionado = "Total";
        private string establecimientoSeleccionado;
        private string tituloGrafico = String.Empty;
        private List<string> nombresEstablecimientos = new();
        private string inputEstablecimiento;



        [CascadingParameter] public FiltrosInforme Filtros { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            await ActualizarGrafico();
        }




        private async Task CambiarEstablecimiento(string nombre)
        {
            establecimientoSeleccionado = nombre;
            await ActualizarGrafico();
        }

        ////Comprobar que establecimiento se selecciona (todos o algun nombre del foreach)
        //private void ValidarYSeleccionarEstablecimiento(ChangeEventArgs e)
        //{
        //    inputEstablecimiento = e.Value?.ToString();

        //    if (string.IsNullOrWhiteSpace(inputEstablecimiento) || inputEstablecimiento == "Todos")
        //    {
        //        CambiarEstablecimiento(null);
        //    }
        //    else if (nombresEstablecimientos.Contains(inputEstablecimiento))
        //    {
        //        CambiarEstablecimiento(inputEstablecimiento);
        //    }
        //    else
        //    {
        //        // Opción inválida escrita a mano: podrías ignorarla, mostrar mensaje, o limpiar el input
        //        CambiarEstablecimiento(null);
        //    }
        //}



        //private async Task CambiarFiltro(string filtro)
        //{
        //    filtroSeleccionado = filtro;
        //    await ActualizarGrafico();
        //}

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
            string establecimiento = Filtros.EstablecimientoSeleccionado;
            bool soloTrimestre = Filtros.FiltroSeleccionado != "Total";
            // ... resto del código igual

            EstablecimientoDatosDTO item;

            if (string.IsNullOrWhiteSpace(establecimientoSeleccionado) || establecimientoSeleccionado == "Todos")
            {
                item = await EstablecimientoService.ObtenerDatosDeTodosLosEstablecimientos(soloTrimestre);
                tituloGrafico = "Bonos canjeados + Importe de todos los establecimientos";
            }
            else
            {
                if (soloTrimestre)
                {
                    item = await EstablecimientoService.ObtenerDatosUltimoTrimestrePorEstablecimiento(establecimientoSeleccionado);
                    tituloGrafico = "Bonos canjeados + Importe en el trimestre activo";
                }
                else
                {
                    item = await EstablecimientoService.ObtenerDatosPorEstablecimiento(establecimientoSeleccionado);
                    tituloGrafico = "Bonos canjeados + Importe por establecimiento";
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
