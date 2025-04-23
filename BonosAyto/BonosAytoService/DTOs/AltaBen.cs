using System.ComponentModel.DataAnnotations;

public class AltaBen
{
    [Required(ErrorMessage = "Falta el nombre")]
    public string Nombre { get; set; }
    [Required(ErrorMessage = "Falta el primer apellido")]
    public string PrimerApellido { get; set; }
    [Required(ErrorMessage = "Falta el segundo apellido")]
    public string SegundoApellido { get; set; }
    [Required(ErrorMessage = "Falta el DNI")]
    [RegularExpression(@"^\d{8}[A-Z]$", ErrorMessage = "DNI inválido")]
    public string DNI { get; set; }
    [Required(ErrorMessage = "Falta la dirección")]
    public string Direccion { get; set; }
    [Required(ErrorMessage = "Falta el código postal")]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "Código postal inválido")]
    public int CodigoPostal { get; set; }
    [Required(ErrorMessage = "Falta el teléfono")]
    [Phone(ErrorMessage = "Teléfono inválido")]
    public string Telefono { get; set; }
    [Required(ErrorMessage = "Se necesita una dirección de correo")]
    [EmailAddress(ErrorMessage = "Dirección de correo inválido")]
    public string Email { get; set; }

    public void reset()
    {
        Nombre = "";
        PrimerApellido = "";
        SegundoApellido = "";
        DNI = "";
        CodigoPostal = 0;
        Direccion = "";
        Telefono = "";
        Email = "";
    }
}
