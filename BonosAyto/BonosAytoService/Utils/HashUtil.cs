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

        public static string GenerarCodigoAlfanumerico(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var code = new StringBuilder();
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];

            rng.GetBytes(bytes);

            for (int i = 0; i < length; i++)
            {
                int index = bytes[i] % chars.Length;
                code.Append(chars[index]);
            }

            return code.ToString();
        }
    }
}
