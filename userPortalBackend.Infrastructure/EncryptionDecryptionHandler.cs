using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace userPortalBackend.Infrastructure
{
    public static class EncryptionDecryptionHandler
    {
        private static readonly string Key = "your-encryption-key_parul@282003";

        public static string Encryption(string data)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = new byte[16]; // Initialize to zero

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new System.IO.MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new System.IO.StreamWriter(cs))
                {
                    sw.Write(data);
                    sw.Close();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
            //if (data == null)
            //{
            //    return null;
            //}
            //byte[] toStoreData = ASCIIEncoding.ASCII.GetBytes(data);
            //string encryptedData = Convert.ToBase64String(toStoreData);
            //return encryptedData;
        }

        public static string Decryption(string encryptedText)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Key);
                aes.IV = new byte[16]; // Initialize to zero

                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new System.IO.MemoryStream(Convert.FromBase64String(encryptedText)))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new System.IO.StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
            //if (data == null)
            //{
            //    return null;
            //}

            //byte[] encryptedData = Convert.FromBase64String(data);
            //string decryptedData = ASCIIEncoding.ASCII.GetString(encryptedData);
            //return decryptedData;
        }
    }
}
