using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.DTOs
{
    public partial class CanjeoDTO
    {
        public int Id { get; set; }
        public int IdBono { get; set; }
        public int IdEstablecimiento { get; set; }
        public DateTime? FechaCanjeo { get; set; }
        public bool OpExitosa { get; set; }
        public string? DescripcionError { get; set; }
        public int UsuarioMod { get; set; }
        public DateTime FechaMod { get; set; }
    }
}
