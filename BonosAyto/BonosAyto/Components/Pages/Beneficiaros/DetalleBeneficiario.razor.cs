using BonosAytoService.DTOs;
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
        private AltaBen detalleValid;
        public AsignarBono bonoAsig;
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

        private IEnumerable<BonoDTO> listaBonos;
        [Inject]
        private IJSRuntime JS { get; set; }




        private string tituloDetalleBeneficiario { get; set; }
        protected override void OnInitialized()
        {
            var uri = Navigation.ToAbsoluteUri(Navigation.Uri);
            var queryParams = QueryHelpers.ParseQuery(uri.Query);
            listaBonos = bonoService.Listar(Id);

            if (queryParams.TryGetValue("edit", out var editValue))
            {
                edit = bool.TryParse(editValue, out var result) && result;
            }

            detalleB = beneficiarioService.Consultar(Id);
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
        private void ModificarBeneficiario()         //modificar beneficiarios           
        {
            detalleB.Nombre = detalleValid.Nombre;
            detalleB.PrimerApellido = detalleValid.PrimerApellido;
            detalleB.SegundoApellido = detalleValid.SegundoApellido;
            detalleB.DNI = detalleValid.DNI;
            detalleB.Direccion = detalleValid.Direccion;
            detalleB.Email = detalleValid.Email;
            detalleB.CodigoPostal = detalleValid.CodigoPostal;
            detalleB.Telefono = detalleValid.Telefono;

            beneficiarioService.Actualizar(detalleB);
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
            listaBonos = bonoService.Listar(Id);

            BonoDTO bonoDTO = new BonoDTO
            {
                IdBeneficiario = Id,
                TipoServicio = bonoAsig.TipoServicio,
                FechaInicio = bonoAsig.FechaInicio,
                FechaCaducidad = bonoAsig.FechaCaducidad,
                Importe = bonoAsig.Importe + "€",
                Activados = bonoAsig.Activados,
                Canjeados = 0,
                Caducados = 0
            };

            int nuevoId = bonoService.Insertar(bonoDTO);
            listaBonos = bonoService.Listar(Id);
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
        public async Task FiltrarTri() {
            if (filtT) {
                filtT = false;
                listaBonos = bonoService.Listar(Id);
            }
            else
            {
                filtT = true;
                //filtrar
                listaBonos=bonoService.ListarFiltT(Id);
            }
        }

        private async Task Borrar(int id)
        {
            bool confirmed = await JS.InvokeAsync<bool>("confirm", $"¿Está seguro de que desea borrar este talonario?");
            if (confirmed)
            {
                bonoService.Eliminar(id);
                listaBonos = bonoService.Listar(Id);
            }
        }
    }
}
