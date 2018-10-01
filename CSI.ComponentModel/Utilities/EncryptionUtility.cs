using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CSI.Utilities
{
    public static class EncryptionUtility
    {
        public static string Decrypt(byte[] encryptedBytes, byte[] key, byte[] iv)
        {
            RijndaelManaged managed = new RijndaelManaged();
            MemoryStream stream = new MemoryStream();
            using (CryptoStream stream2 = new CryptoStream(stream, managed.CreateDecryptor(key, iv), CryptoStreamMode.Write))
            {
                stream2.Write(encryptedBytes, 0, encryptedBytes.Length);
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        public static byte[] Encrypt(string value, byte[] key, byte[] iv)
        {
            RijndaelManaged managed = new RijndaelManaged();
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            MemoryStream stream = new MemoryStream();
            using (CryptoStream stream2 = new CryptoStream(stream, managed.CreateEncryptor(key, iv), CryptoStreamMode.Write))
            {
                stream2.Write(bytes, 0, bytes.Length);
            }
            return stream.ToArray();
        }

        public static byte[] GenerateIV()
        {
            RijndaelManaged managed = new RijndaelManaged();
            managed.GenerateIV();
            return managed.IV;
        }

        public static byte[] GenerateKey()
        {
            RijndaelManaged managed = new RijndaelManaged();
            managed.GenerateKey();
            return managed.Key;
        }
    }
}

