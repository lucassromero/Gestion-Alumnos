using GestionProfesora.Api.Data;
using GestionProfesora.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace GestionProfesora.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddDbContext<GestionProfesoraDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly("GestionProfesora.Api")));

        builder.Services.AddSingleton<ICurrentProfesora, CurrentProfesoraService>();
        builder.Services.AddScoped<IAlumnoService, AlumnoService>();
        builder.Services.AddScoped<IMateriaService, MateriaService>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("FrontendDev", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173", "https://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors("FrontendDev");

        // En desarrollo, demo y tests el frontend usa HTTP sin TLS local.
        // Evita redirecciones a HTTPS y el warning por falta de puerto HTTPS.
        if (!app.Environment.IsDevelopment() &&
            !app.Environment.IsEnvironment("Demo") &&
            !app.Environment.IsEnvironment("Testing"))
        {
            app.UseHttpsRedirection();
        }

        app.MapControllers();

        if (app.Environment.IsEnvironment("Demo"))
        {
            await DemoDataSeeder.SeedAsync(app.Services);
        }

        await app.RunAsync();
    }
}
