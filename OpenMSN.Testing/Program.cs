using Microsoft.EntityFrameworkCore;
using OpenMSN.Data;
using OpenMSN.Data.Entities;

class Program
{
    public static void Main(string[] args)
    {
        ApplicationDbContext dbContext = new();

        var contacts = dbContext.Contacts
            .Include(x => x.User)
            .Include(x => x.TargetUser)
            .ThenInclude(x => x.Contacts)
            .Where(x => x.UserId == 1)
            .ToList();
    }
}
