using Microsoft.AspNetCore.Components;
using BonosAytoService.DAOs;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.JSInterop;
using BonosAytoService.Model;




namespace BonosAyto.Components.Pages.Beneficiaros
{
    public partial class ListadoAltaBeneficiarios
    {
        AltaBen modeloAlta = new AltaBen(); //clase de validacion

        private BeneficiarioService beneficiarioService = new BeneficiarioService();

        private IEnumerable<BeneficiarioDTO> listaBeneficiarios;

        private string fichero = "Sin selección";

        [Inject]
        private IJSRuntime JS { get; set; }

        protected override void OnInitialized() //cargar lista
        {
            listaBeneficiarios = beneficiarioService.Listar();
        }

        private async Task AltaBeneficiario()         //dar de alta beneficiarios           
        {
            

            BeneficiarioDTO ben = new BeneficiarioDTO
            {
                Nombre = modeloAlta.Nombre,
                PrimerApellido = modeloAlta.PrimerApellido,
                SegundoApellido = modeloAlta.SegundoApellido,
                DNI = modeloAlta.DNI,
                Direccion = modeloAlta.Direccion,
                Email = modeloAlta.Email,
                CodigoPostal = modeloAlta.CodigoPostal,
                Telefono = modeloAlta.Telefono
            };
           
            int nuevoId = beneficiarioService.Insertar(ben);
            listaBeneficiarios = beneficiarioService.Listar(); 
            modeloAlta.reset();
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

        //botones accion
        private void VerDetalle(int Id)
        {
            Navigate.NavigateTo($"/beneficiarios/detallebeneficiario/{Id}?edit=false");
        }
        private void Modificar(int Id)
        {
            Navigate.NavigateTo($"/beneficiarios/detallebeneficiario/{Id}?edit=true");
        }
        private async Task Borrar(int Id)
        {
            bool confirmed = await JS.InvokeAsync<bool>("confirm", $"¿Está seguro de que desea eliminar al beneficiario?");
            if (confirmed)
            {
                var eliminado = beneficiarioService.Eliminar(Id);
                listaBeneficiarios = beneficiarioService.Listar();
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

                    var nuevosBeneficiarios = new List<BeneficiarioDTO>();

                    int currentRow = 2;

                    foreach (var row in rows)
                    {
                        var alta = new AltaBen
                        {
                            Nombre = row.Cell(1).GetString().Trim(),
                            PrimerApellido = row.Cell(2).GetString().Trim(),
                            SegundoApellido = row.Cell(3).GetString().Trim(),
                            DNI = row.Cell(4).GetString().Trim(),
                            Direccion = row.Cell(5).GetString().Trim(),
                            CodigoPostal = int.TryParse(row.Cell(6).GetString().Trim(), out var cp) ? cp : 0,
                            Telefono = row.Cell(7).GetString().Trim(),
                            Email = row.Cell(8).GetString().Trim()
                        };

                        var context = new ValidationContext(alta);
                        var results = new List<ValidationResult>();

                        bool isValid = Validator.TryValidateObject(alta, context, results, true);

                        if (isValid)
                        {
                            nuevosBeneficiarios.Add(new BeneficiarioDTO
                            {
                                Nombre = alta.Nombre,
                                PrimerApellido = alta.PrimerApellido,
                                SegundoApellido = alta.SegundoApellido,
                                DNI = alta.DNI,
                                Direccion = alta.Direccion,
                                CodigoPostal = alta.CodigoPostal,
                                Telefono = alta.Telefono,
                                Email = alta.Email
                            });
                        }

                        currentRow++;
                    }
                    foreach (var b in nuevosBeneficiarios)
                    {
                        beneficiarioService.Insertar(b);
                    }
                    listaBeneficiarios = beneficiarioService.Listar();
                    StateHasChanged();

                    mensajeError = $"Se cargaron {nuevosBeneficiarios.Count} beneficiarios correctamente.";
                }
                catch (Exception ex)
                {
                    mensajeError = $"Error al procesar el archivo, beneficiarios ya registrados";
                }
            }
            else
            {
                mensajeError = "Por favor, selecciona un archivo Excel antes de cargar.";
            }
        }
    }
}
