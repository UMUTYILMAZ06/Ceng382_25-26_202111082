using LabProject.Models;
using Microsoft.EntityFrameworkCore;

namespace LabProject.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
            : base(options)
        {
        }

        public DbSet<Class> Classes { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
    }
}
