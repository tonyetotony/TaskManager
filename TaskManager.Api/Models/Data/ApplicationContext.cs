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
using TaskManager.Api.Models.Data;

namespace TaskManager.Api.Models.Data;

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ProjectAdmin> ProjectsAdmins { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Desk> Desks { get; set; }
    public DbSet<Task> Tasks { get; set; } // Изменил на TaskEntity, чтобы избежать конфликта с ключевым словом "Task"

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        // Не используем EnsureCreated() здесь
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Здесь можно добавлять конфигурации моделей
    }
}
