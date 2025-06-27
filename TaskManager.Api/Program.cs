/*
using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models.Data;

var builder = WebApplication.CreateBuilder(args);
// получаем строку подключения из файла конфигурации
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connectionString));
var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () =>
{

});

await app.RunAsync();
*/

using Microsoft.EntityFrameworkCore;
using TaskManager.Api.Models;
using TaskManager.Api.Models.Data;

var builder = WebApplication.CreateBuilder(args);

// Получаем строку подключения
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Регистрируем контекст БД
builder.Services.AddDbContext<ApplicationContext>(options => 
    options.UseSqlServer(connectionString));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", () =>
{
    
});

// Добавляем seed-данные только один раз (при запуске)
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetService<ApplicationContext>();

if (context != null && !context.Users.Any(u => u.Status == UserStatus.Admin))
{
    var admin = new User("anton", "pasichnichenko", "admin", "qwerty123", UserStatus.Admin);
    context.Users.Add(admin);
    context.SaveChanges();
}

await app.RunAsync();
