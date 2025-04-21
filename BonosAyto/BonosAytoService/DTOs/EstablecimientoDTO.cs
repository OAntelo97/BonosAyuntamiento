using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.DTOs
{
    public class EstablecimientoDTO
    {
        public int Id { get; set; }
        public string NIF { get; set; }
        public string Direccion { get; set; }
        public int CodigoPostal { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public int UsuarioMod { get; set; }
        public DateTime FechaMod { get; set; }
    }
}
