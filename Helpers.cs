using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace timw255.Sitefinity.Moodle
{
    public static class Helpers
    {
        public static Guid ToGuid(string src)
        {
            byte[] stringbytes = Encoding.UTF8.GetBytes(src);
            byte[] hashedBytes = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(stringbytes);
            Array.Resize(ref hashedBytes, 16);
            return new Guid(hashedBytes);
        }

        public static byte[] xorIt(byte[] key, byte[] data)
        {
            byte[] result = new byte[16];

            key.CopyTo(result, 0);

            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(key[i] ^ data[i]);
            }

            return result;
        }
    }
}
