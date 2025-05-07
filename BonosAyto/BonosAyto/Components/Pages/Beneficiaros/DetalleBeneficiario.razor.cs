using BonosAytoService.DTOs;
using System.Threading.Tasks;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components;
using BonosAytoService.DAOs;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using ClosedXML.Excel;
using Microsoft.JSInterop;
using DocumentFormat.OpenXml.EMMA;


namespace BonosAyto.Components.Pages.Beneficiaros
{
    public partial class DetalleBeneficiario
    {
        private BeneficiarioDTO detalleB = new BeneficiarioDTO();
        private AltaBen detalleValid = new AltaBen();
        public AsignarBono bonoAsig = new AsignarBono();
        private BeneficiarioService beneficiarioService = new BeneficiarioService();
        private BonoService bonoService = new BonoService();
        [Parameter]
        public int Id { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }
        [Parameter]
        public bool edit { get; set; }
        EditContext bonoContext;
        ValidationMessageStore messageStore;
        private int IdElimunar = 0;
        private string MensajeErrorEliminar = "";
        private string tituloError = "";

        private IEnumerable<BonoDTO> listaBonos = [];
        [Inject]
        private IJSRuntime JS { get; set; } 
        private string tituloDetalleBeneficiario { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            listaBonos = await bonoService.ConsultarPorBeneficiario(Id);

            if (queryParams.TryGetValue("edit", out var editValue))
            {
                edit = bool.TryParse(editValue, out var result) && result;
            }

            detalleB = await beneficiarioService.Consultar(Id);
            detalleValid = new AltaBen
            {
                Nombre = detalleB.Nombre,
                PrimerApellido = detalleB.PrimerApellido,
                SegundoApellido = detalleB.SegundoApellido,
                DNI = detalleB.DNI,
                Direccion = detalleB.Direccion,
                Email = detalleB.Email,
                CodigoPostal = detalleB.CodigoPostal,
                Telefono = detalleB.Telefono
            };
            bonoAsig = new AsignarBono
            {
                TipoServicio = ' ',
                FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0),
                FechaCaducidad = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddMonths(3),
                Importe = 3,
                Activados = 20
            };
            bonoContext = new EditContext(bonoAsig);
            messageStore = new ValidationMessageStore(bonoContext);

            titulo();
        }
        private async Task ModificarBeneficiario()         //modificar beneficiarios           
        {
            detalleB.Nombre = detalleValid.Nombre;
            detalleB.PrimerApellido = detalleValid.PrimerApellido;
            detalleB.SegundoApellido = detalleValid.SegundoApellido;
            detalleB.DNI = detalleValid.DNI;
            detalleB.Direccion = detalleValid.Direccion;
            detalleB.Email = detalleValid.Email;
            detalleB.CodigoPostal = detalleValid.CodigoPostal;
            detalleB.Telefono = detalleValid.Telefono;

            int res = await beneficiarioService.Actualizar(detalleB);

            // Comprobar si el DNI o Email del beneficiario ya existe
            if (res <= 0)
            {
                tituloError = "actualizar";
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

            titulo();
        }

        private void titulo()
        {
            tituloDetalleBeneficiario = $"Información de {detalleB.Nombre} {detalleB.PrimerApellido} {detalleB.SegundoApellido}";
        }

        private void AutoCaducidad()
        {
            bonoAsig.FechaCaducidad = bonoAsig.FechaInicio.AddMonths(3);
        }

        private void CheckCaducidad()
        {
            if (bonoAsig.FechaCaducidad <= bonoAsig.FechaInicio)
            {
                bonoAsig.FechaCaducidad = bonoAsig.FechaInicio.AddMonths(3);
            }
        }


        private async Task AgregarBono()
        {
            listaBonos = await bonoService.ConsultarPorBeneficiario(Id);

            BonoDTO bonoDTO = new BonoDTO
            {
                IdBeneficiario = Id,
                TipoServicio = bonoAsig.TipoServicio,
                FechaInicio = bonoAsig.FechaInicio,
                FechaCaducidad = bonoAsig.FechaCaducidad,
                Importe = bonoAsig.Importe,
                Activados = bonoAsig.Activados,
                Canjeados = 0,
                Caducados = 0
            };

            int nuevoId = await bonoService.Insertar(bonoDTO);
            listaBonos = await bonoService.ConsultarPorBeneficiario(Id);
            bonoAsig.reset();
        }

        public string TipoServicioNombre(char c)
        {
            switch (c)
            {
                case 'R': return "Restaurante";
                default: return "Tipo inválido";
            }
        }

        //botones accion
        private void VerDetalle(int id)
        {
            Navigate.NavigateTo($"/bonos/detalletalonario/{id}?edit=false");
        }
        private void Modificar(int id)
        {
            Navigate.NavigateTo($"/bonos/detalletalonario/{id}?edit=true");
        }

        private bool filtT = false;
        public async Task FiltrarTri()
        {
            if (filtT)
            {
                filtT = false;
                listaBonos = await bonoService.ConsultarPorBeneficiario(Id);
            }
            else
            {
                filtT = true;
                //filtrar
                listaBonos = await bonoService.ListarFiltT(Id);
            }
        }

        private async Task Borrar(int id)
        {
            int res = await bonoService.Eliminar(id);
            if (res <= 0)
            {
                tituloError = "eliminar";
                switch (res)
                {
                    case -2:
                        MensajeErrorEliminar = "Se ha producido un error debido a que está intentado borrar un bono del cual ya se realizó uno o más canjeos. " +
                            "Por favor, asegúrese de que este bono no tenga relación con ningún canjeo antes de borrarlo.";
                        break;
                    default:
                        MensajeErrorEliminar = "Se ha producido un error inesperado. Por favor, vuelva a intentarlo mas tarde";
                        break;
                }
                AbrirModal("EliminarError");
            }
            listaBonos = await bonoService.ConsultarPorBeneficiario(Id);
        }

        private async Task AbrirModal(string modalId)
        {
            await JS.InvokeVoidAsync("MODAL.AbrirModal", modalId);
        }
    }
}