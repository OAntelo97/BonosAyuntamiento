using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BonosAytoService.DTOs;
using BonosAytoService.Services;

namespace BonosAytoTest
{
    public class BonoTest
    {
        [Fact]
        public void Test1()
        {
            /************ TESTS DE INSCRIPCIONES ***********/
            /*Test de INSERTAR*/
            var service = new BonoService();
            var bonoDTO = new BonoDTO
            {
                AlumnoId = 3,
                MateriaId = 2
            };

            int nuevoId = service.Insertar(inscripcionDTO);

            Assert.True(nuevoId > 0);



            /*Test de consultar*/
            var inscripcionObtenida = service.Consultar(nuevoId);
            Assert.NotNull(inscripcionObtenida);
            Assert.Equal(3, inscripcionObtenida?.AlumnoId);



            /*actualizacion*/
            inscripcionObtenida.AlumnoId = 2;
            inscripcionObtenida.MateriaId = 3;
            var actualizado = service.Actualizar(inscripcionObtenida);
            Assert.True(actualizado);




            /**/
            var todos = service.Listar().ToList();
            Assert.Contains(todos, a => a.Id == nuevoId && a.AlumnoId == 2);





            var eliminado = service.Eliminar(nuevoId);
            Assert.True(eliminado);

            var inscripcionEliminada = service.Consultar(nuevoId);
            Assert.Null(inscripcionEliminada);
        }
    }
}
