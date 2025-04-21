using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.DTOs
{
    internal class BeneficiarioDTO
    {
        private int Id { get; set; }
        private string Nombre { get; set; }
        private string PrimerApellido { get; set; }
        private string SegundoApellido { get; set; }
        private string DNI { get; set; }
        private string Direccion { get; set; }
        private int CodigoPostal { get; set; }
        private string Telefono { get; set; }
        private string Email { get; set; }
        private int UsuarioMod { get; set; }
        private DateTime FechaMod { get; set; }
    }
}
