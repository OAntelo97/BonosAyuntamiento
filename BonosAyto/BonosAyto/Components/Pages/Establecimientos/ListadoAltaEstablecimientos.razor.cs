using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using BonosAyto.Components.Modales;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace BonosAyto.Components.Pages.Establecimientos
{
    public partial class ListadoAltaEstablecimientos
    {
        /******************FORM DE ALTAS*****************/
        [Inject]
        private EstablecimientoService EstablecimientoService { get; set; }
        [Inject]
        private UsuarioService UsuarioService { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthStateTask { get; set; }
        private string fichero = "Sin selección";

        public bool Confirmation { get; set; } = false;

        private EstablecimientoDTO establecimiento = new EstablecimientoDTO();
        private AlertaModal Modal { get; set; }

        private int IdElimunar = 0;

        private async Task GuardarEstablecimiento()
        {
           // Si no existe, insertar
            int nuevoId = await EstablecimientoService.Insertar(establecimiento);
            establecimientos = await EstablecimientoService.Listar(); //asi se recarga la lista despues de insertar para que los nuevos registros se muestren en la tabla tmb, y no solo cuando se haga f5
            establecimiento = new EstablecimientoDTO();
        }






        /******************LISTADO*****************/
        private IEnumerable<EstablecimientoDTO> establecimientos = [];

        protected async override Task OnInitializedAsync()
        {
            // Obtener todas las inscripciones usando el servicio y un ienumerable
            establecimientos = await EstablecimientoService.Listar();
        }

        //protected override async Task OnInitializedAsync()
        //{
        //    var authState = await AuthStateTask;

        //    GlobalVariables.usuario = UsuarioService.Consultar(Int32.Parse(authState.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
        //}



        private void VerDetalle(int id)
        {
            //Navigate.NavigateTo($"/establecimientos/ver/{id}");
        }
        private void Editar(int id)
        {
            //Navigate.NavigateTo($"/establecimientos/editar/{id}");
        }
        private async Task Eliminar(int id)
        {
            Confirmation = true;
            //int res =  await EstablecimientoService.Eliminar(id);
            //if(res == -2)
            //{

            //}else if(res == -3)
            //{

            //}else if (res == -1)
            //{

            //}
            //// Actualizar la lista después de eliminar la inscripción
            //establecimientos = await EstablecimientoService.Listar();
        }



        private string mensajeError = null;
        private IBrowserFile fExcel = null;
        private void SeleccionarFichero(InputFileChangeEventArgs e)  //seleccionar fichero, solo permite xlsx
        {
            var file = e.File;

            if (file != null)
            {
                if (file.Name.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    fichero = file.Name;
                    fExcel = file;
                    mensajeError = null;
                }
                else
                {
                    fichero = null;
                    fExcel = null;
                    mensajeError = "Por favor selecciona un archivo Excel .xlsx";
                }
            }
        }

        private async Task CargarExcel()  //cargar datos de excel
        {
            if (fExcel != null)
            {
                try
                {
                    using var stream = fExcel.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); //recorrer excel
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    using var workbook = new XLWorkbook(memoryStream);
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed().Skip(1);

                    var nuevosEstablecimientos = new List<EstablecimientoDTO>();

                    int currentRow = 2;

                    foreach (var row in rows)  //recorrer
                    {
                        string codpost = row.Cell(4).GetString().Trim();
                        var alta = new EstablecimientoDTO
                        {
                            Nombre = row.Cell(1).GetString().Trim(),
                            NIF = row.Cell(2).GetString().Trim(),
                            Direccion = row.Cell(3).GetString().Trim(),
                            CodigoPostal = codpost == "" ? null : int.TryParse(row.Cell(4).GetString().Trim(), out var cp) ? cp : 0,
                            Telefono = row.Cell(5).GetString().Trim(),
                            Email = row.Cell(6).GetString().Trim()
                        };

                        alta.NIF = alta.NIF != "" ? alta.NIF : null;
                        alta.Direccion = alta.Direccion != "" ? alta.Direccion : null;
                        alta.Telefono = alta.Telefono != "" ? alta.Telefono : null;
                        alta.Email = alta.Email != "" ? alta.Email : null;

                        var context = new ValidationContext(alta);
                        var results = new List<ValidationResult>();

                        bool isValid = Validator.TryValidateObject(alta, context, results, true);  //validar

                        if (isValid)
                        {
                            nuevosEstablecimientos.Add(alta);
                        }

                        currentRow++;
                    }
                    foreach (var b in nuevosEstablecimientos) //insertar
                    {
                        EstablecimientoService.Insertar(b);
                    }
                    establecimientos = await EstablecimientoService.Listar();

                    mensajeError = $"Se cargaron {nuevosEstablecimientos.Count} establecimientos correctamente.";
                }
                catch (Exception ex)
                {
                    mensajeError = $"Error al procesar el archivo, establecimientos ya registrados";
                }
            }
            else
            {
                mensajeError = "Por favor, selecciona un archivo Excel antes de cargar.";
            }
        }
    }
}
