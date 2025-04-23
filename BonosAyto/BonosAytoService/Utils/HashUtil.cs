using System.Security.Cryptography;
using System.Text;

namespace BonosAytoService.Utils
{
    public static class HashUtil
    {
        public static string ObtenerHashSHA256(string cadena)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(cadena);

            byte[] hashValue = SHA256.HashData(messageBytes);

            return Convert.ToHexString(hashValue);
        }


    }
}
