using System.Security.Cryptography;
using System.Text;

namespace API_BUSESCONTROL.Helpers {
    public static class Criptografia {

        public static string GerarHash(this string value) {
            var hash = SHA1.Create();
            var encod = new ASCIIEncoding();
            var array = encod.GetBytes(value);

            array = hash.ComputeHash(array);

            var strHexa = new StringBuilder();

            foreach (var item in array) {
                strHexa.Append(item.ToString("x2"));
            }
            return strHexa.ToString();
        }

    }
}
