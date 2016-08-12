namespace Darc.Framework.Extensions
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public static class CryptoExtension
    {
        // This constant string is used as a "salt" value for the PasswordDeriveBytes function calls.
        // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
        // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
        private const string initVector = "tuac234i110889u2";

        // This constant is used to determine the keysize of the encryption algorithm.
        private const int KeySize = 256;

        public static string Encrypt(this string clearText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(clearText);

            var password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(KeySize/8);
            var symmetricKey = new RijndaelManaged();

            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.Zeros;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);

            using (var memoryStream = new MemoryStream())
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();

                return Convert.ToBase64String(cipherTextBytes);
            }
        }

        public static string Decrypt(this string cipherText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

            var password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(KeySize / 8);

            var symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.Zeros;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);

            using (var memoryStream = new MemoryStream(cipherTextBytes))
            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                var plainTextBytes = new byte[cipherTextBytes.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
        }
    }
}