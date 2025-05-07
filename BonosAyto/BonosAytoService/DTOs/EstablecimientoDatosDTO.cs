using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.DTOs
{
    public partial class EstablecimientoDatosDTO
    {
        public string NombreEstablecimiento { get; set; }
        public int BonosCanjeados { get; set; }
        public decimal ImporteTotal { get; set; }
    }
}
