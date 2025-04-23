using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.DTOs
{
    public partial class UsuarioDTO
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Pass { get; set; }
        public char Rol { get; set; }

        [EmailAddress(ErrorMessage = "Formato de correo no válido!")]
        public string Email { get; set; }
        public int? IdEstablecimiento { get; set; }
        public int UsuarioMod { get; set; } = 1;
        public DateTime FechaMod { get; set; }
    }
}
