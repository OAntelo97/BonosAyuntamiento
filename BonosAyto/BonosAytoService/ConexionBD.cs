using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BonosAytoService
{
    public static class ConexionBD
    {
        private static string _configuration;

        public static void Inicilizar(String conexion)
        {
            _configuration = conexion;
        }

        public static string CadenaDeConexion()
        {
            return _configuration;
        }
    }
}
