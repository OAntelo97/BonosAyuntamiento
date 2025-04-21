using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BonosAytoService.DTOs;
using BonosAytoService.Services;

namespace BonosAytoTest
{
    public class EstablecimientoTest
    {
        [Fact]
        public void Test1() {
            var service = new EstablecimientoService();
            var establecimientoDTO = new EstablecimientoDTO
            {
                Nombre = "Galsoft",
                NIF = "1346234253D"
            };

            int nuevoId = service.Insertar(establecimientoDTO);

            Assert.True(nuevoId > 0);

            var clineteObtenido = service.Consultar(nuevoId);
            Assert.NotNull(clineteObtenido);
            Assert.Equal("Galsoft", clineteObtenido?.Nombre);

            clineteObtenido.Nombre = "Galsoft2";
            var avtualizado = service.Actualizar(clineteObtenido);
            Assert.True(avtualizado);

            var todos = service.Listar().ToList();

            Assert.Contains(todos, c => c.Id == nuevoId && c.Nombre == "Galsoft2");


            var eliminado = service.Eliminar(nuevoId);
            Assert.True(eliminado);

            var establecimientoEliminado = service.Consultar(nuevoId);
            Assert.Null(establecimientoEliminado);
        }
    }
}
