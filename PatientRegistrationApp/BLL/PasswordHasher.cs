using System;
using System.Security.Cryptography;

namespace PatientRegistrationApp.BLL
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bits
        private const int KeySize = 32;  // 256 bits
        private const int Iterations = 100_000;

        // returns a stored hash in the format: iterations.salt.hash
        public static string Hash(string password)
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            byte[] key;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                key = pbkdf2.GetBytes(KeySize);
            }

            return string.Format("{0}.{1}.{2}",
                Iterations,
                Convert.ToBase64String(salt),
                Convert.ToBase64String(key));
        }

        // verifies the password against the stored hash
        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash))
                return false;

            var parts = storedHash.Split(new[] { '.' }, 3);
            if (parts.Length != 3)
                return false;

            int iterations;
            if (!int.TryParse(parts[0], out iterations))
                return false;

            byte[] salt;
            byte[] expectedKey;
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                expectedKey = Convert.FromBase64String(parts[2]);
            }
            catch
            {
                return false;
            }

            byte[] actualKey;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                actualKey = pbkdf2.GetBytes(expectedKey.Length);
            }

            return FixedTimeEquals(actualKey, expectedKey);
        }

        // compares two byte arrays in a way that is resistant to timing attacks
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int diff = 0;
            for (int i = 0; i < a.Length; i++)
            {
                diff |= a[i] ^ b[i];
            }
            return diff == 0;
        }
    }
}
