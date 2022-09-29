using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;

namespace OpenMSN.Data.Entities
{
    [Index(nameof(EmailAddress), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        public string EmailAddress { get; set; } = null!;
        public bool Activated { get; set; } = false;
        public string ActivationToken { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string PasswordHashMD5 { get; set; } = null!;
        public string MD5Salt { get; set; } = null!;
        public long TimeCreated { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        public bool CanLogin()
        {
            if (!Activated)
                return false;

            return true;
        }

        public void SetPassword(string password)
        {
            PasswordHash = Argon2.Hash(password);

            // so, MSNP2 - MSNP7 requires the use of MD5 authentication
            //
            // it goes a bit something like this:
            //  > the server sends the client a salt
            //  > the client sends the server an md5 hash in the form of password+salt
            //  > the server authenticates based on that
            //
            // back then, a different salt would've been sent every time (usually the current unix timestamp)
            // which only really works if you store the password in plaintext
            //
            // the way i'll be doing it here is by generating a static authentication salt for each user
            // that way the md5 hash is always known to the server, and so it can be safely stored with argon2

            // generate md5 salt (random 20-char length hex string)
            byte[] buffer = new byte[10];
            new Random().NextBytes(buffer);
            MD5Salt = Convert.ToHexString(buffer);

            // generate argon2-md5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password + MD5Salt);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                string md5_digest = Convert.ToHexString(hashBytes);
                PasswordHashMD5 = Argon2.Hash(md5_digest);
            }
        }

        public bool VerifyPassword(string password)
        {
            return Argon2.Verify(PasswordHash, password);
        }

        public bool VerifyPasswordMD5(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(password + MD5Salt);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                string md5_digest = Convert.ToHexString(hashBytes);
                return Argon2.Verify(PasswordHashMD5, md5_digest);
            }
        }
    }
}
