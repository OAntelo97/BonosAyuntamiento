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
        private BarChart<double> barChartDias;
        private BarChart<double> barChartMensual;
        private List<string> nombresEstablecimientos = new();
        private string establecimientoSeleccionado;
        private string filtroSeleccionado = "Total";
        private string tituloGrafico = String.Empty;
        private string inputEstablecimiento;


        // Se actualiza en cada cambio
        private async Task CambiarEstablecimiento(string nombre)
        {
            establecimientoSeleccionado = nombre;
            await ActualizarGrafico();
        }

        //Comprobar que establecimiento se selecciona (todos o algun nombre del foreach)
        private void ValidarYSeleccionarEstablecimiento(ChangeEventArgs e)
        {
            inputEstablecimiento = e.Value?.ToString();

            if (string.IsNullOrWhiteSpace(inputEstablecimiento) || inputEstablecimiento == "Todos")
            {
                CambiarEstablecimiento(null);
            }
            else if (nombresEstablecimientos.Contains(inputEstablecimiento))
            {
                CambiarEstablecimiento(inputEstablecimiento);
            }
            else
            {
                // Opción inválida escrita a mano: podrías ignorarla, mostrar mensaje, o limpiar el input
                CambiarEstablecimiento(null);
            }
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

            bool soloTrimestre = filtroSeleccionado != "Total";

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


            await ActualizarGraficoDias(establecimientoSeleccionado, soloTrimestre);
            await ActualizarGraficoMensual();
        }





        private async Task ActualizarGraficoDias(string establecimiento, bool soloTrimestre)
        {
            if (barChartDias != null)
            {
                await barChartDias.Clear();
            }

            Dictionary<int, (int Bonos, double Importe)> datos;

            if (string.IsNullOrWhiteSpace(establecimiento) || establecimiento == "Todos")
            {
                // Obtener datos de todos los establecimientos
                datos = await EstablecimientoService.ObtenerBonosEImportePorDiaSemanaTodos(soloTrimestre);
            }
            else
            {
                // Obtener datos para el establecimiento seleccionado
                datos = await EstablecimientoService.ObtenerBonosEImportePorDiaSemana(establecimiento, soloTrimestre);
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





        private async Task ActualizarGraficoMensual()
        {
            if (barChartMensual == null)
                return;

            await barChartMensual.Clear();

            bool soloTrimestre = filtroSeleccionado == "Trimestre activo";

            Dictionary<int, (int Bonos, double Importe)> datos;

            if (string.IsNullOrWhiteSpace(establecimientoSeleccionado) || establecimientoSeleccionado == "Todos")
            {
                datos = await EstablecimientoService.ObtenerBonosEImportePorMesTodos(soloTrimestre);
                // Iterar sobre el diccionario y mostrar los datos
                Console.WriteLine("DATOS:");
                foreach (var item in datos)
                {
                    Console.WriteLine($"Mes: {item.Key}, Bonos: {item.Value.Bonos}, Importe: {item.Value.Importe}");
                }
            }
            else
            {
                datos = await EstablecimientoService.ObtenerBonosEImportePorMes(establecimientoSeleccionado, soloTrimestre);
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

            // Verificar el estado final de los datos antes de actualizar el gráfico
            Console.WriteLine("Bonos (lista final):");
            foreach (var bono in bonos)
            {
                Console.WriteLine(bono);
            }

            Console.WriteLine("Importes (lista final):");
            foreach (var importe in importes)
            {
                Console.WriteLine(importe);
            }

            // Verificar las listas que van al gráfico
            Console.WriteLine("Bonos (lista final):");
            foreach (var bono in bonos)
            {
                Console.WriteLine(bono);
            }

            Console.WriteLine("Importes (lista final):");
            foreach (var importe in importes)
            {
                Console.WriteLine(importe);
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

            await barChartMensual.Update();
        }




    }
}
