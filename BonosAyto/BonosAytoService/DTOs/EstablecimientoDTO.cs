using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.DTOs
{
    public class EstablecimientoDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obligatorio. Porfavor, introduzca el Nombre")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Campo obligatorio. Porfavor, introduzca el NIF")]
        [RegularExpression(@"^[A-Z]\d{8}$", ErrorMessage = "NIF no válido")]
        public string? NIF { get; set; }
        public string? Direccion { get; set; }
        [RegularExpression(@"^\d{5}$", ErrorMessage = "Código postal inválido")]
        public int? CodigoPostal { get; set; }
        public string? Telefono { get; set; }
        [EmailAddress(ErrorMessage = "Dirección de correo no válida")]
        public string? Email { get; set; }
        public int UsuarioMod { get; set; }
        public DateTime FechaMod { get; set; }
    }
}
