using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Isopoh.Cryptography.Argon2;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using MailKit.Security;
using MailKit.Net.Smtp;

namespace OpenMSN.Data.Entities
{
    [Index(nameof(EmailAddress), IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        public bool Activated { get; set; } = false;
        public string ActivationToken { get; set; } = Guid.NewGuid().ToString();
        public string Username { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string PasswordHashMD5 { get; set; } = null!;
        public string MD5Salt { get; set; } = null!;
        public DateTimeOffset TimeCreated { get; set; } = DateTimeOffset.UtcNow;

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
            //  > the client sends the server an md5 hash in the form of salt+password
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
            MD5Salt = Convert.ToHexString(buffer).ToLower();

            // generate argon2-md5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(MD5Salt + password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                string md5_digest = Convert.ToHexString(hashBytes).ToLower();
                PasswordHashMD5 = Argon2.Hash(md5_digest);
            }
        }

        public bool VerifyPassword(string password)
        {
            return Argon2.Verify(PasswordHash, password);
        }

        public bool VerifyPasswordMD5(string password)
        {
            //using (MD5 md5 = MD5.Create())
            //{
            //    byte[] inputBytes = Encoding.ASCII.GetBytes(password);
            //    byte[] hashBytes = md5.ComputeHash(inputBytes);

            //    string md5_digest = Convert.ToHexString(hashBytes);
            //    return Argon2.Verify(PasswordHashMD5, md5_digest);
            //}

            return Argon2.Verify(PasswordHashMD5, password);
        }

        //public async Task SendEmail(string subject, TextPart body)
        //{
        //    MimeMessage message = new();

        //    message.From.Add(MailboxAddress.Parse("no-reply@openmsn.pizzaboxer.xyz"));
        //    message.To.Add(MailboxAddress.Parse(EmailAddress));

        //    message.Subject = subject;
        //    message.Body = body;

        //    using (SmtpClient smtp = new())
        //    {
        //        await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        //        await smtp.AuthenticateAsync("no-reply@openmsn.pizzaboxer.xyz", "");
                
        //        await smtp.SendAsync(message);
        //        await smtp.DisconnectAsync(true);
        //    }
        //}
    }
}
