using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace StarterApp.Database.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // This is the same connection string used at runtime
        // 10.0.2.2 is the Docker host address from inside the Android emulator
        optionsBuilder.UseNpgsql(
            "Host=localhost:5432;Username=app_user;Password=app_password;Database=appdb"
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}