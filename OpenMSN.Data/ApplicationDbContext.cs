using Microsoft.EntityFrameworkCore;
using OpenMSN.Data.Entities;

namespace OpenMSN.Data
{
    public class ApplicationDbContext : DbContext
    {
        //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        //    : base(options) 
        //{ 
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("host=localhost;port=5432;database=openmsn;user id=postgres;password=root");
            optionsBuilder.UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // this doesn't load the target user entity for the first result for some reason
            //modelBuilder.Entity<Contact>()
            //    .HasOne(x => x.TargetUser)
            //    .WithOne()
            //    .HasForeignKey<Contact>(x => x.TargetUserId);

            // this loads the wrong contacts for the target user entity
            //modelBuilder.Entity<Contact>()
            //    .HasOne(x => x.TargetUser)
            //    .WithMany(x => x.Contacts)
            //    .HasForeignKey(x => x.TargetUserId);

            modelBuilder.Entity<Contact>()
                .HasOne(x => x.TargetUser)
                .WithMany()
                .HasForeignKey(x => x.TargetUserId);

#if DEBUG
            // passwords for both test accounts are just "password"
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Activated = true,
                    ActivationToken = "78196fd1-8181-45d0-97c8-16a59977acb4",
                    Username = "pizzaboxer@hotmail.com",
                    EmailAddress = "pizzaboxer@hotmail.com",
                    PasswordHash = "$argon2id$v=19$m=65536,t=3,p=1$/+34CRzCS6MJv30NTB1fDQ$IoisuDjna+EYXU617erIYmXMAU6/NJdQeXx7XA+w8L0",
                    PasswordHashMD5 = "$argon2id$v=19$m=65536,t=3,p=1$GHWJXxxgaAz1gNFOZTeHkg$BSni28HhYC9Dx1LcVTnmyJBl4GEPR0YcfPxWTmO0hyc",
                    MD5Salt = "7f30143936ba04972090"
                },
                new User
                {
                    UserId = 2,
                    Activated = true,
                    ActivationToken = "63e9f88f-8f89-4470-a329-6f29d8b15a49",
                    Username = "pizzaboxer2@hotmail.com",
                    EmailAddress = "pizzaboxer2@hotmail.com",
                    PasswordHash = "$argon2id$v=19$m=65536,t=3,p=1$rfVcCqUoJNQEXNFcoD/KtA$tC0Sik/4zzCN3WSMnG6L6fHFAc2I+QeRGmjREZMsOkM",
                    PasswordHashMD5 = "$argon2id$v=19$m=65536,t=3,p=1$F/NwmzVUbf2BkmONdub81g$NW9o0CvFt/H3forXfcHKSWxNRdgirel9px5S6l1IpzQ",
                    MD5Salt = "6502408dbcbea1e56eeb"
                }
            );
#endif
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Contact> Contacts { get; set; } = null!;
    }
}
