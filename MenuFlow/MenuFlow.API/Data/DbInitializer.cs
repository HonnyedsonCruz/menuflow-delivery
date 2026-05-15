using MenuFlow.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuFlow.API.Data;

public static class DbInitializer
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        var existeAdmin = await context.Usuarios.AnyAsync();

        if (existeAdmin)
            return;

        var admin = new Usuario
        {
            Nome = "Admin MenuFlow",
            Email = "admin@menuflow.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "Admin",
            CriadoEm = DateTime.UtcNow
        };

        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();
    }
}