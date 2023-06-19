
using System.Security.Cryptography;
using System.Text;


namespace Service.Service
{
    public static class UtilityService
    {

        public static string ConvertHA256(string text)
        {
            string hash = string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {

                //OBETENR EL HASH DEL TEXTO RECIBIDO 

                byte[] hashvalue = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));


                //CONVERTIR EL ARRAY BYTE EN CADENA DE TEXTO

                foreach (byte b in hashvalue)
                {
                    hash += $"{b:X2}";
                }
                return hash;
            }

        
        }

        public static string GenerateToken()
        {

            string token = Guid.NewGuid().ToString("N");

            return token;
        }
    }
}
