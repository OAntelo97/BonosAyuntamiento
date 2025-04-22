using BonosAytoService.Services;

namespace BonosAyto.Components.Pages.Beneficiaros
{
    public partial class DetalleBeneficiario
    {
        private string datos = "";
        private string materias = "";
        public int Id { get; set; }

        BeneficiarioService beneficiarioService =new BeneficiarioService();

        public DetalleBeneficiario() { 
            datos=beneficiarioService.Consultar(Id).Nombre;
        }
    }
}
