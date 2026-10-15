/*using Microsoft.EntityFrameworkCore;

namespace TaskManager.Api.Models.Data;

public class ApplicationContext: DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ProjectAdmin> ProjectsAdmins { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Desk> Desks { get; set; }
    public DbSet<Task> Tasks { get; set; }

    public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
    {
        Database.EnsureCreated();
        if (Users.Any(u => u.Status == UserStatus.Admin) == false)
        {
            var admin = new User("anton", "pasichnichenko", "admin", "qwerty123",UserStatus.Admin);
            Users.Add(admin);
            SaveChanges();
        }
    }


}*/

using Microsoft.EntityFrameworkCore;
using TaskManager.Common.Models;

namespace TaskManager.Api.Models.Data;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ProjectAdmin> ProjectsAdmins { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Desk> Desks { get; set; }
    public DbSet<Task> Tasks { get; set; } 

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
        if (Users.Any(u => u.Status == UserStatus.Admin) == false)
        {
            
            var admin = new User("anton", "pasichnichenko", "admin",
                "qwerty123", UserStatus.Admin, "+79991234567");
            Users.Add(admin);
            SaveChanges();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Настраиваем связь Desk - Task
        modelBuilder.Entity<Desk>()
            .HasMany(d => d.Tasks)
            .WithOne(t => t.Desk)
            .HasForeignKey(t => t.DeskId);
    }
}
