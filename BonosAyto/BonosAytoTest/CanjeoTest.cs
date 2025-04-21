using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BonosAytoService.DTOs;
using BonosAytoService.Services;

namespace BonosAytoTest
{
    public class CanjeoTest
    {
        [Fact]
        public void Test1()
        {
            /************ TESTS DE INSCRIPCIONES ***********/
            /*Test de INSERTAR*/
            var service = new CanjeoService();
            var canjeoDTO = new CanjeoDTO
            {
                IdBono = 1,
                IdEstablecimiento = 2,
                FechaCanjeo = DateTime.Now,
                OpExitosa = true,
                DescripcionError = "",
                UsuarioMod = 2,
                FechaMod = DateTime.Now
            };

            int nuevoId = service.Insertar(canjeoDTO);

            Assert.True(nuevoId > 0);



            /*Test de consultar*/
            var canjeoObtenido = service.Consultar(nuevoId);
            Assert.NotNull(canjeoObtenido);
            Assert.Equal(3, canjeoObtenido?.Id);



            /*actualizacion*/
            canjeoObtenido.IdBono = 2;
            canjeoObtenido.IdEstablecimiento = 3;
            var actualizado = service.Actualizar(canjeoObtenido);
            Assert.True(actualizado);




            /**/
            var todos = service.Listar().ToList();
            Assert.Contains(todos, a => a.Id == nuevoId && a.IdEstablecimiento == 3);





            var eliminado = service.Eliminar(nuevoId);
            Assert.True(eliminado);

            var canjeoEliminado = service.Consultar(nuevoId);
            Assert.Null(canjeoEliminado);
        }
    }
}
