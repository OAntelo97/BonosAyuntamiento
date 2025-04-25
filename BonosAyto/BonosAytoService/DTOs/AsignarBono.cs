using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService.DTOs
{
    public class AsignarBono
    {
        public class CalidadValidar : ValidationAttribute
        {
            private readonly string _fechaInicio;

            public CalidadValidar(string fechaInicioPropertyName)
            {
                _fechaInicio = fechaInicioPropertyName;
                ErrorMessage = "La fecha de caducidad debe ser mayor que la fecha de inicio.";
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var fechaCaducidad = value as DateTime? ?? default;

                var fecha = validationContext.ObjectType.GetProperty(_fechaInicio);
                if (fecha == null)
                {
                    return new ValidationResult($"Propiedad no encontrada: {_fechaInicio}");
                }

                var fechaInicio = fecha.GetValue(validationContext.ObjectInstance) as DateTime? ?? default;

                if (fechaCaducidad <= fechaInicio)
                {
                    return new ValidationResult(ErrorMessage);
                }
                return ValidationResult.Success;
            }
        }



        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        protected class ServicioValidator : ValidationAttribute
        {
            public ServicioValidator(string error)
            {
                ErrorMessage = error;
            }

            public override bool IsValid(object value)
            {
                char c = (char)value;
                return c!= ' ';
            }
        }


        [ServicioValidator("Falta el tipo de servicio")]        
        public char TipoServicio { get; set; }
        [Required(ErrorMessage = "Falta la fecha de inicio")]
        public DateTime FechaInicio { get; set; }
        [Required(ErrorMessage = "Falta la fecha de caducidad")]
        [CalidadValidar("FechaInicio")]
        public DateTime FechaCaducidad { get; set; }
        public int Importe { get; set; }
        [Required(ErrorMessage = "Falta la cantidad de Importes")]
        [Range(1, int.MaxValue, ErrorMessage ="Cantidad incorrecta")]
        public int Activados { get; set; }
        
    

        public void reset()
        {
            TipoServicio = ' ';
            FechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            FechaCaducidad = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).AddMonths(3);
            Activados = 20;
            Importe = 3;
        }
    }

    
}
