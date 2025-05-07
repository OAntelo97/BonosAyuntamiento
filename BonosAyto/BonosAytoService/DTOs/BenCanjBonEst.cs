using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.DTOs
{
    public class BenCanjBonEst
    {
        public EstablecimientoDTO establecimiento { get; set; }
        public CanjeoDTO canjeo { get; set; }
        public BonoDTO bono { get; set; }
        public BeneficiarioDTO beneficiario { get; set; }
    }
}