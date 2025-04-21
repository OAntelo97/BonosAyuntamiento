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
                IdBeneficiario = 3, 
                TipoServicio = 'C', 
                FechaInicio = DateTime.Now,
                FechaCaducidad = DateTime.Now.AddMonths(1),
                Importe = "100",
                Activados = 20,
                Canjeados = 0,
                Caducados = 0,
                UsuarioMod = 1,
                FechaMod = DateTime.Now
            };

            int nuevoId = service.Insertar(bonoDTO);

            Assert.True(nuevoId > 0);



            /*Test de consultar*/
            var bonoObtenido = service.Consultar(nuevoId);
            Assert.NotNull(bonoObtenido);
            Assert.Equal(3, bonoObtenido?.Id);



            /*actualizacion*/
            bonoObtenido.IdBeneficiario = 2;
            bonoObtenido.Importe = "200";
            var actualizado = service.Actualizar(bonoObtenido);
            Assert.True(actualizado);




            /**/
            var todos = service.Listar().ToList();
            Assert.Contains(todos, a => a.Id == nuevoId && a.IdBeneficiario == 2);





            var eliminado = service.Eliminar(nuevoId);
            Assert.True(eliminado);

            var bonoEliminado = service.Consultar(nuevoId);
            Assert.Null(bonoEliminado);
        }
    }
}
