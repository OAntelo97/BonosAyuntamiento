using BonosAytoService.DTOs;
using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;

namespace BonosAyto.Components.Pages.Beneficiaros
{
    public partial class DetalleBeneficiario
    {
        private BeneficiarioDTO detalleB = new BeneficiarioDTO();
        private AltaBen detalleValid;
        private BeneficiarioService beneficiarioService = new BeneficiarioService();
        [Parameter]
        public int Id { get; set; }
        private string tituloDetalleBeneficiario {  get; set; }
        protected override void OnInitialized() 
        {
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

        private void titulo() { 
            tituloDetalleBeneficiario = $"Información de {detalleB.Nombre} {detalleB.PrimerApellido} {detalleB.SegundoApellido}";
        }
    }
}
