using IdentityService.Database;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}