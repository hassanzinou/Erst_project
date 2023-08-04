using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SQLExecuter
{
   public class Crypter
    {

        public static string Encrypt(string plainText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream();

            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);

            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

            cryptoStream.FlushFinalBlock();

            byte[] cipherTextBytes = memoryStream.ToArray();

            memoryStream.Close();
            cryptoStream.Close();

            var cipherText = Convert.ToBase64String(cipherTextBytes);

            return cipherText;
        }
        public static string Decrypt(string cipherText, string passPhrase, string saltValue, string hashAlgorithm, int passwordIterations, string initVector, int keySize)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);

            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            RijndaelManaged symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;

            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

            byte[] plainTextBytes = new byte[cipherTextBytes.Length + 1];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

            memoryStream.Close();
            cryptoStream.Close();

            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);

            return plainText;
        }

        public static string DecryptConfigValue(ref ConfigFile configFile, string caption, string entry, string passphrase)
        {
            string result = configFile.GetValue(caption, entry, true);

            if (!string.IsNullOrEmpty(passphrase) && !string.IsNullOrEmpty(result))
            {
                if (result.ToUpper().StartsWith("{MD5}"))
                    result = Crypter.Decrypt(result.Substring(5).Trim(), passphrase.ToLower(), passphrase.ToUpper(), "MD5", 3, "123456789ABCDEFG", 256);
                else if (result.ToUpper().StartsWith("{SHA1}"))
                    result = Crypter.Decrypt(result.Substring(6).Trim(), passphrase.ToLower(), passphrase.ToUpper(), "SHA1", 3, "123456789ABCDEFG", 256);
                else
                {
                    string newValue = "{SHA1} " + Crypter.Encrypt(result.Trim(), passphrase.ToLower(), passphrase.ToUpper(), "SHA1", 3, "123456789ABCDEFG", 256);
                    configFile.SetValue(caption, entry, newValue, true);
                    configFile.Save();
                }
            }

            return result;
        }
    }
}
