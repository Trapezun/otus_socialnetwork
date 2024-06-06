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

        static public (byte[] hash, byte[] salt) GetHashPassword(string password)
        {
            using (Rfc2898DeriveBytes rdb = new Rfc2898DeriveBytes(password, saltSize, iterations))
            {                               
                var sh = rdb.GetBytes(hashSize);                
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
      
    }
}