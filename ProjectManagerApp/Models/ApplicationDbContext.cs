using Microsoft.EntityFrameworkCore;

namespace ProjectManagerApp.Models
{
    /// <summary>
    /// Třída slouží jako databázový kontext pro propojení s tabulkami databáze v rámci EF Core frameworku
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; } // Tabulka projektů
        public DbSet<Employee> Employees { get; set; } // Tabulka zaměstnanců
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
