using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SocialNetwork.Classes
{
    public static class HashPasswordHelper
    {
        private const int iterations = 1000;
        private const int saltSize = 16;
        private const int hashSize = 16;

        static public (byte[] hashed, byte[] salt) GetHashPassword(string password)
        {
            using (Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(password, saltSize, iterations))
            {               
                //var salt = getString(rdb.Salt);                              
                var sh = rdb.GetBytes(hashSize);
                //var hash = getString(sh);               
                //return (hash, salt);

                return (sh, rdb.Salt);



            }
        }

      
        static public bool ValidatePassword(string password, byte[] storedSalt, byte[] storedHash)
        {
            if (storedSalt == null) return false;

            bool retVal = false;
            using (Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(password, storedSalt, iterations))
            {
                byte[] hash = rdb.GetBytes(hashSize);
                var hsh = storedHash;
                retVal = hsh.SequenceEqual(hash);
            }
            return retVal;

        }


        static byte[] getBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string getString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);


        }
    }
}