using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace userPortalBackend.Infrastructure
{
    public static class EncryptionDecryptionHandler
    {
        public static string Encryption(string data)
        {
            if (data == null)
            {
                return null;
            }
            byte[] toStoreData = ASCIIEncoding.ASCII.GetBytes(data);
            string encryptedData = Convert.ToBase64String(toStoreData);
            return encryptedData;
        }

        public static string Decryption(string data)
        {
            if (data == null)
            {
                return null;
            }

            byte[] encryptedData = Convert.FromBase64String(data);
            string decryptedData = ASCIIEncoding.ASCII.GetString(encryptedData);
            return decryptedData;
        }
    }
}
