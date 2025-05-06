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
                return c != ' ';
            }
        }


        [ServicioValidator("Falta el tipo de servicio")]
        public char TipoServicio { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaCaducidad { get; set; }
        public int Importe { get; set; }
        [Required(ErrorMessage = "Falta la cantidad de Importes")]
        [Range(1, int.MaxValue, ErrorMessage = "Cantidad incorrecta")]
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