using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.Model
{
    public class Bono
    {
        public int Id { get; set; }
        public int IdBeneficiario { get; set; }
        public char TipoServicio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaCaducidad { get; set; }
        public string Importe { get; set; }
        public int Activados { get; set; } = 20;
        public int Canjeados { get; set; } = 0;
        public int Caducados { get; set; } = 0;
        public int UsuarioMod { get; set; }
        public DateTime FechaMod { get; set; }



    }
}
