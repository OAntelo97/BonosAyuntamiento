using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.Model
{
    public class Beneficiario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }
        public string DNI { get; set; }
        public string Direccion { get; set; }
        public int CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public int UsuarioMod { get; set; }
        public DateTime FechaMod { get; set; }
    }
}
