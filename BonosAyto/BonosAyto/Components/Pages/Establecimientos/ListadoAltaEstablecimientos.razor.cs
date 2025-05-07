using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using BonosAyto.Components.Modales;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
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

        private string tituloError = "";
        private string MensajeErrorEliminar = "";

        private EstablecimientoDTO establecimiento = new EstablecimientoDTO();

        private int IdElimunar = 0;

        private async Task GuardarEstablecimiento()
        {
           // Si no existe, insertar
            int nuevoId = await EstablecimientoService.Insertar(establecimiento);
            tituloError = "insertar";
            if (nuevoId <= 0)
            {
                tituloError = "insertar";
                switch (nuevoId)
                {
                    case -2:
                        MensajeErrorEliminar = "Ya existe un usuario con este NIF.";
                        break;
                    case -3:
                        MensajeErrorEliminar = "Ya existe un usuario con este correo.";
                        break;
                    default:
                        MensajeErrorEliminar = "Se ha producido un error inesperado. Por favor, vuelva a intentarlo mas tarde";
                        break;
                }
                AbrirModal("EliminarError");
                return;
            }
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


        private void VerDetalle(int id)
        {
            Navigate.NavigateTo($"/establecimientos/ver/{id}");
        }
        private void Editar(int id)
        {
            Navigate.NavigateTo($"/establecimientos/editar/{id}");
        }
        private async Task Eliminar(int id)
        {
            
            int res = await EstablecimientoService.Eliminar(id);
            if (res <=0)
            {
                tituloError = "eliminar";
                switch (res)
                {
                    case -2:
                        MensajeErrorEliminar = "Se ha producido un error debido a que está intentado borrar un establecimiento el cual esta asignado a un usuario. " +
                            "Por favor, asegúrese de que este establecimiento no tenga relación con ningún usuario antes de borrarlo.";
                        break;
                    case -3:
                        MensajeErrorEliminar = "Se ha producido un error debido a que está intentado borrar un establecimiento en el cual se realizaron canjeos de bonos. " +
                            "Por favor, asegúrese de que este establecimiento no tenga relación con ningún canjeo antes de borrarlo.";
                        break;
                    default:
                        MensajeErrorEliminar = "Se ha producido un error inesperado. Por favor, vuelva a intentarlo mas tarde";
                        break;
                }
                AbrirModal("EliminarError");
            }
            
            // Actualizar la lista después de eliminar la inscripción
            establecimientos = await EstablecimientoService.Listar();
        }
        
        private async Task AbrirModal(string modalId)
        {
            await JS.InvokeVoidAsync("MODAL.AbrirModal", modalId);
        }

        private List<string> mensajeError = [];
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
                    List<string> NIFErrors = [];
                    List<string> EmailErrors = [];
                    int numEstablecimientos = nuevosEstablecimientos.Count();
                    foreach (var b in nuevosEstablecimientos) //insertar
                    {
                        int res = await EstablecimientoService.Insertar(b);
                        // Comprobar si el nombre de usuario ya
                        if (res < -1)
                        {
                            if (res == -2)
                            {
                                NIFErrors.Add(b.NIF);
                            }
                            if (res == -3)
                            {
                                EmailErrors.Add(b.Email);

                            }
                            numEstablecimientos--;
                        }
                    }
                    establecimientos = await EstablecimientoService.Listar();

                    mensajeError.Add($"Se cargaron {numEstablecimientos} establecimientos correctamente.");
                    if (NIFErrors.Count + EmailErrors.Count > 0)
                    {
                        mensajeError.Add($"Hubo {NIFErrors.Count + EmailErrors.Count} establecimientos que fallaron al insertarse.");
                        if (NIFErrors.Count > 0)
                        {
                            mensajeError.Add($"{NIFErrors.Count} establecimientos tienen un NIF que ya se encuentra en la base de datos: {string.Join(", ", NIFErrors)}");
                        }
                        if (EmailErrors.Count > 0)
                        {
                            mensajeError.Add($"{EmailErrors.Count} establecimientos tienen un Email que ya se encuentra en la base de datos: {string.Join(", ", EmailErrors)}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    mensajeError.Add($"Error al procesar el archivo, establecimientos ya registrados");
                }
            }
            else
            {
                mensajeError.Add("Por favor, selecciona un archivo Excel antes de cargar.");
            }
        }
    }
}
