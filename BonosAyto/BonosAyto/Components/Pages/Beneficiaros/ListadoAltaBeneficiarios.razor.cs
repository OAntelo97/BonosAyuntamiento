using Microsoft.AspNetCore.Components;
using BonosAytoService.DAOs;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using ClosedXML.Excel;
using System.Threading.Tasks;
using BonosAytoService.Models;
using Microsoft.JSInterop;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.JSInterop;




namespace BonosAyto.Components.Pages.Beneficiaros
{
    public partial class ListadoAltaBeneficiarios
    {
        AltaBen modeloAlta = new AltaBen(); //clase de validacion

        private BeneficiarioService beneficiarioService = new BeneficiarioService();

        private IEnumerable<BeneficiarioDTO> listaBeneficiarios = [];
        private List<BeneficiarioDTO> beneficiariosFiltrados = new();
        [Inject]
        private IJSRuntime JS { get; set; }
        private string searchbar = "";
        private string fichero = "Sin selección";
        private int IdElimunar = 0;
        private string MensajeErrorEliminar = ""; 
        private string tituloError = "";
        private void FiltrarBeneficiarios() //filtrar lista
        {
            
            if (string.IsNullOrWhiteSpace(searchbar))
            {
                beneficiariosFiltrados = listaBeneficiarios.ToList();
            }
            else
            {
                beneficiariosFiltrados = listaBeneficiarios
                    .Where(b => (b.Nombre + " " + b.PrimerApellido + " " + b.SegundoApellido).ToLower().Contains(searchbar.ToLower())
                             || b.Telefono.Contains(searchbar)
                             || b.Email.ToLower().Contains(searchbar.ToLower()))
                    .ToList();
                StateHasChanged();
            }
        }


        protected override async Task OnInitializedAsync() //cargar lista
        {
            listaBeneficiarios = await beneficiarioService.Listar();
            beneficiariosFiltrados = listaBeneficiarios.ToList();
        }

        private async Task AltaBeneficiario()         //dar de alta beneficiarios           
        {                                       
            BeneficiarioDTO ben = new BeneficiarioDTO     //Cambiar UsuarioMod
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
            ;
            int res = await beneficiarioService.Insertar(ben);
            // Comprobar si el DNI o Email del beneficiario ya existe
            if (res <= 0)
            {
                tituloError = "insertar";
                switch (res)
                {
                    case -2:
                        MensajeErrorEliminar = "Ya existe un beneficiario con este DNI.";
                        break;
                    case -3:
                        MensajeErrorEliminar = "Ya existe un beneficiario con este correo.";
                        break;
                    default:
                        MensajeErrorEliminar = "Se ha producido un error inesperado. Por favor, vuelva a intentarlo mas tarde";
                        break;
                }
                AbrirModal("EliminarError");
                return;
            }
            listaBeneficiarios = await beneficiarioService.Listar();
            FiltrarBeneficiarios();
            modeloAlta.reset();
        }

        private List<string> mensajeError = [];
        private bool cargando = false;
        private IBrowserFile fExcel = null;
        private void SeleccionarFichero(InputFileChangeEventArgs e)  //seleccionar fichero, solo permite xlsx
        {
            var file = e.File;

            if (file != null)
            {
                mensajeError = [];
                if (file.Name.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    fichero = file.Name;
                    fExcel = file;
                }
                else
                {
                    fichero = null;
                    fExcel = null;
                    mensajeError.Add("Por favor selecciona un archivo Excel .xlsx");
                }
            }
        }

        //botones accion
        private void VerDetalle(int Id)
        {
            Navigate.NavigateTo($"/beneficiarios/ver/{Id}");
        }
        private void Modificar(int Id)
        {
            Navigate.NavigateTo($"/beneficiarios/editar/{Id}");
        }
        private async Task Borrar(int Id)
        {
            int res = await beneficiarioService.Eliminar(Id);
            if (res <= 0)
            {
                tituloError = "eliminar";
                switch (res)
                {
                    case -2:
                        MensajeErrorEliminar = "Se ha producido un error debido a que está intentado borrar un beneficiario el cual tiene asignados uno o más bonos. " +
                            "Por favor, asegúrese de que este beneficiario no tenga relación con ningún bono antes de borrarlo.";
                        break;
                    default:
                        MensajeErrorEliminar = "Se ha producido un error inesperado. Por favor, vuelva a intentarlo mas tarde";
                        break;
                }
                AbrirModal("EliminarError");
            }
            listaBeneficiarios = await beneficiarioService.Listar();
            FiltrarBeneficiarios();
        }
        private async Task AbrirModal(string modalId)
        {
            await JS.InvokeVoidAsync("MODAL.AbrirModal", modalId);
        }

        private async Task CargarExcel() //cargar datos de excel
        {
            if (cargando)
            {
                return;
            }else cargando = true;
            mensajeError = [];
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
                    List<string> DNIErrors= [];
                    List<string> EmailErrors =[];
                    int numBeneficiarios = nuevosBeneficiarios.Count();
                    foreach (var b in nuevosBeneficiarios) //insertar
                    {
                        int res = await beneficiarioService.Insertar(b);
                        // Comprobar si el nombre de usuario ya
                        if (res <-1)
                        {
                            if (res == -2)
                            {
                                DNIErrors.Add(b.DNI);
                            }
                            if (res == -3)
                            {
                                EmailErrors.Add(b.Email);
                                
                            }
                            numBeneficiarios--;
                        }
                    }
                    listaBeneficiarios =  await beneficiarioService.Listar();
                    FiltrarBeneficiarios();

                    mensajeError.Add($"Se cargaron {numBeneficiarios} beneficiarios correctamente.");
                    if (DNIErrors.Count + EmailErrors.Count > 0)
                    {
                        mensajeError.Add($"Hubo {DNIErrors.Count + EmailErrors.Count} beneficiarios que fallaron al insertarse.");
                        if (DNIErrors.Count > 0)
                        {
                            mensajeError.Add($"{DNIErrors.Count} beneficiarios tienen un DNI que ya se encuentra en la base de datos: {string.Join(", ",DNIErrors)}");
                        }
                        if (EmailErrors.Count > 0) {
                            mensajeError.Add($"{EmailErrors.Count} beneficiarios tienen un Email que ya se encuentra en la base de datos: {string.Join(", ", EmailErrors)}");
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    mensajeError.Add($"Error al procesar el archivo, beneficiarios ya registrados");
                }
            }
            else
            {
                mensajeError.Add("Por favor, selecciona un archivo Excel antes de cargar.");
            }
            cargando = false;
        }
    }
}
