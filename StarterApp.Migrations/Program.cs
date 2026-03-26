using Microsoft.EntityFrameworkCore;
using StarterApp.Database.Data;

Console.WriteLine("Running migrations...");

var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseNpgsql(
    "Host=localhost:5432;Username=app_user;Password=app_password;Database=appdb"
);

using var context = new AppDbContext(optionsBuilder.Options);
context.Database.Migrate();

Console.WriteLine("Migrations complete.");
