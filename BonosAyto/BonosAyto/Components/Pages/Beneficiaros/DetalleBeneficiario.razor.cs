using BonosAytoService.Services;
using Microsoft.AspNetCore.Components;

namespace BonosAyto.Components.Pages.Beneficiaros
{
    public partial class DetalleBeneficiario
    {
        private string datos = "";
        private string materias = "";

        [Parameter]
        public int Id { get; set; }

        private BeneficiarioService beneficiarioService = new BeneficiarioService();

        protected override void OnInitialized()
        {
            var beneficiario = beneficiarioService.Consultar(Id);
            if (beneficiario != null)
            {
                datos = beneficiario.Nombre;
                materias = ""; 
            }
        }
    }
}
