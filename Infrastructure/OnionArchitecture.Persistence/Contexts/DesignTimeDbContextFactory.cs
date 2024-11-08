using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OnionArchitecture.Persistence.Contexts;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OnionArchitectureDbContext>
{
    public OnionArchitectureDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<OnionArchitectureDbContext> dbContextBuilder = new();

        // Use UseSqlServer method for MsSQL connection
        dbContextBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=D-Voice;Trusted_Connection=True;");
        //dbContextBuilder.UseSqlServer("data source=UZNKAYA\\SQLEXPRESS; initial catalog=D-Voice; integrated security=true;TrustServerCertificate=True");

        return new OnionArchitectureDbContext(dbContextBuilder.Options);
    }
}
