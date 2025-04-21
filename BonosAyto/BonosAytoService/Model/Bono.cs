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
        public int Activados { get; set; }
        public int Canjeados { get; set; }
        public int Caducados { get; set; }
        public int UsuarioMod { get; set; }
        public DateTime FechaMod { get; set; }



    }
}
