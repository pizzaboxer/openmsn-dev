using Microsoft.EntityFrameworkCore;
using OpenMSN.Data.Entities;

namespace OpenMSN.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options) 
        { 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("host=localhost;port=5432;database=openmsn;user id=postgres;password=root");
            optionsBuilder.UseSnakeCaseNamingConvention();
        }

        public DbSet<User> Users { get; set; }
    }
}
