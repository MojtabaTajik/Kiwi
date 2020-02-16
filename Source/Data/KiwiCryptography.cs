using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

namespace Data
{
    public class KiwiCryptography
    {
        public static string HashPassword(string password)
        {
            const string passwordSalt = "H~$U&@!#sd#RE&(@JDe(#^Ry8ej|'";
            var passwordSaltBytes = Encoding.UTF8.GetBytes(passwordSalt);

            byte[] hashedBytes = KeyDerivation.Pbkdf2(
                password: password,
                salt: passwordSaltBytes,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 1000,
                numBytesRequested: 256 / 8
            );

            return BitConverter.ToString(hashedBytes).Replace("-", string.Empty);
        }
    }
}