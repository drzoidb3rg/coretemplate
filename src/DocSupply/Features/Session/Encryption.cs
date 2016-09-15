using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DocSupply.Features.Session
{
    public class Encryption
    {
        private static int SaltSize()
        {
            return 16;
        }

        private static byte[] GetSalt()
        {
             return Guid.NewGuid().ToByteArray();
        }

        public static string Encrypt(string plainText, string password )
        {
            if (plainText == null)
                throw new ArgumentNullException("plainText");
            if (password == null)
                throw new ArgumentNullException("password");

            // Will return the cipher text
            string cipherText = "";

            // Utilizes helper function to generate random 16 byte salt using RNG
            byte[] salt = GetSalt();

            // Convert plain text to bytes
            byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);

            // create new password derived bytes using password/salt
            using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt))
            {

                using (Aes aes = Aes.Create())
                {
                    // Generate key and iv from password/salt and pass to aes
                    aes.Key = pdb.GetBytes(aes.KeySize / 8);
                    aes.IV = pdb.GetBytes(aes.BlockSize / 8);

                    // Open a new memory stream to write the encrypted data to
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Create a crypto stream to perform encryption
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            // write encrypted bytes to memory
                            cs.Write(plainBytes, 0, plainBytes.Length);
                        }
                        // get the cipher bytes from memory
                        byte[] cipherBytes = ms.ToArray();
                        // create a new byte array to hold salt + cipher
                        byte[] saltedCipherBytes = new byte[salt.Length + cipherBytes.Length];
                        // copy salt + cipher to new array
                        Array.Copy(salt, 0, saltedCipherBytes, 0, salt.Length);
                        Array.Copy(cipherBytes, 0, saltedCipherBytes, salt.Length, cipherBytes.Length);
                        // convert cipher array to base 64 string
                        cipherText = Convert.ToBase64String(saltedCipherBytes);
                    }
                   // aes.Clear();
                }
            }
            return cipherText;
        }

        public static string Decrypt(string cipherText, string password)
        {
            if (cipherText == null)
                throw new ArgumentNullException("cipherText");
            if (password == null)
                throw new ArgumentNullException("password");

            // will return plain text
            string plainText = "";
            // get salted cipher array
            byte[] saltedCipherBytes = Convert.FromBase64String(cipherText);
            // create array to hold salt
            byte[] salt = new byte[SaltSize()];
            // create array to hold cipher
            byte[] cipherBytes = new byte[saltedCipherBytes.Length - salt.Length];

            // copy salt/cipher to arrays
            Array.Copy(saltedCipherBytes, 0, salt, 0, salt.Length);
            Array.Copy(saltedCipherBytes, salt.Length, cipherBytes, 0, saltedCipherBytes.Length - salt.Length);

            // create new password derived bytes using password/salt
            using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt))
            {

                using (Aes aes = Aes.Create())
                {
                    // Generate key and iv from password/salt and pass to aes
                    aes.Key = pdb.GetBytes(aes.KeySize / 8);
                    aes.IV = pdb.GetBytes(aes.BlockSize / 8);

                    // Open a new memory stream to write the encrypted data to
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // Create a crypto stream to perform decryption
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            // write decrypted data to memory
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                        }
                        // convert decrypted array to plain text string
                        plainText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                    //aes.Clear();
                }
            }
            return plainText;
        }
    }
}
