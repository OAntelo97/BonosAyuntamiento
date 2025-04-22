using Microsoft.AspNetCore.Components;
using BonosAytoService.DAOs;
using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Forms;

namespace BonosAyto.Components.Pages.Beneficiaros
{
    public partial class ListadoAltaBeneficiarios
    {
        class AltaBen
        {
            [Required(ErrorMessage = "Falta el nombre")]
            public string Nombre { get; set; }
            [Required(ErrorMessage = "Falta el primer apellido")]
            public string PrimerApellido { get; set; }
            [Required(ErrorMessage = "Falta el segundo apellido")]
            public string SegundoApellido { get; set; }
            [Required(ErrorMessage = "Falta el DNI")]
            [RegularExpression(@"^\d{8}[A-Z]$", ErrorMessage = "DNI inválido")]
            public string DNI { get; set; }
            [Required(ErrorMessage = "Falta la dirección")]
            public string Direccion { get; set; }
            [Required (ErrorMessage ="Falta el código postal")]
            [Range(1, int.MaxValue, ErrorMessage = "Código postal inválido")]
            public int CodigoPostal { get; set; }
            [Required(ErrorMessage = "Falta el teléfono")]
            public string Telefono { get; set; }
            [Required(ErrorMessage = "Se necesita una dirección de correo")]
            [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Dirección de correo inválido")]
            public string Email{ get; set; }

            public void reset()
            {
                Nombre = "";
                PrimerApellido = "";
                SegundoApellido = "";
                DNI = "";
                CodigoPostal = 0;
                Direccion = "";
                Telefono = "";
                Email = "";
            }
        }

        AltaBen modeloAlta = new AltaBen();


        private BeneficiarioService beneficiarioService = new BeneficiarioService();

        private IEnumerable<BeneficiarioDTO> listaBeneficiarios;
        private List<BeneficiarioDTO> beneficiariosFiltrados = new();

        private string searchbar = "";
        private string fichero = "Sin selección";

        private void FiltrarBeneficiarios()
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


        protected override void OnInitialized()
        {
            listaBeneficiarios = beneficiarioService.Listar();
            beneficiariosFiltrados = listaBeneficiarios.ToList();
        }

        private void AltaBeneficiario()                    //Cambiar UsuarioMod
        {
            BeneficiarioDTO ben = new BeneficiarioDTO
            {
                Nombre=modeloAlta.Nombre,
                PrimerApellido =modeloAlta.PrimerApellido,
                SegundoApellido=modeloAlta.SegundoApellido,
                DNI=modeloAlta.DNI,
                Direccion=modeloAlta.Direccion,
                Email=modeloAlta.Email, 
                CodigoPostal=modeloAlta.CodigoPostal,
                Telefono=modeloAlta.Telefono,
                UsuarioMod=0                                //Cambiar UsuarioMod
            };
            beneficiarioService.Insertar(ben);
            listaBeneficiarios = beneficiarioService.Listar();
            FiltrarBeneficiarios();
            modeloAlta.reset();
        }

        private string mensajeError = null;
        private void SeleccionarFichero(InputFileChangeEventArgs e)
        {
            var file = e.File;

            if (file != null)
            {
                if (file.Name.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    fichero = file.Name;
                    mensajeError = null;
                }
                else
                {
                    fichero = null;
                    mensajeError = "Por favor selecciona un archivo Excel (.xls o .xlsx)";
                }
            }
        }
        private void VerDetalle(int Id)
        {
            Navigate.NavigateTo($"/beneficiarios/detallebeneficiario/{Id}");
        }
        private void Modificar(int Id)
        {
            Navigate.NavigateTo($"/beneficiarios/detallebeneficiario/{Id}");
        }
        private void Borrar(int Id)
        {
            beneficiarioService.Eliminar(Id);
            listaBeneficiarios = beneficiarioService.Listar();
            FiltrarBeneficiarios();
        }


        private void CargarExcel() { 
            //carga datos excel a collection, foreach insertar
        }
    }
}
