using System.Security.Cryptography;


namespace Intranet.Core.Helpers
{
    public class PasswordHasher
    {
        private const int SaltSize = 16; // You can adjust the salt size based on your security requirements
        private const int Iterations = 10000; // You can adjust the number of iterations based on your security requirements
        private const int DerivedKeySize = 32; // You can adjust the derived key size based on your security requirements

        public static string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password using PBKDF2
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] derivedKey = pbkdf2.GetBytes(DerivedKeySize);

                // Combine salt and derived key
                byte[] hashBytes = new byte[SaltSize + DerivedKeySize];
                Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
                Buffer.BlockCopy(derivedKey, 0, hashBytes, SaltSize, DerivedKeySize);

                // Convert the combined salt and derived key to a Base64-encoded string
                string hashedPassword = Convert.ToBase64String(hashBytes);
                return hashedPassword;
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            // Extract salt and derived key from the hashed password
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);
            byte[] storedKey = new byte[DerivedKeySize];
            Buffer.BlockCopy(hashBytes, SaltSize, storedKey, 0, DerivedKeySize);

            // Hash the entered password using the extracted salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] enteredKey = pbkdf2.GetBytes(DerivedKeySize);

                // Compare the stored key with the newly computed key
                return ByteArraysEqual(storedKey, enteredKey);
            }
        }

        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                    return false;
            }

            return true;
        }
    }
}
