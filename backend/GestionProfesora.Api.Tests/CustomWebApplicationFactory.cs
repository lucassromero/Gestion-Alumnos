using GestionProfesora.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GestionProfesora.Api.Tests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string DefaultTestConnection =
        "Server=.\\SQLEXPRESS;Database=GestionProfesora_Test;Trusted_Connection=True;TrustServerCertificate=True;";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GestionProfesoraDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<GestionProfesoraDbContext>(options =>
                options.UseSqlServer(GetTestConnectionString()));
        });
    }

    public async Task ResetAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GestionProfesoraDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    Task IAsyncLifetime.InitializeAsync() => ResetAsync();

    Task IAsyncLifetime.DisposeAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GestionProfesoraDbContext>();
        context.Database.EnsureDeleted();
        return Task.CompletedTask;
    }

    private static string GetTestConnectionString()
    {
        return Environment.GetEnvironmentVariable("ConnectionStrings__TestConnection")
            ?? DefaultTestConnection;
    }
}
