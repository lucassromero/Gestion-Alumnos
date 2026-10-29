using GestionProfesora.Api.Data;
using GestionProfesora.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestionProfesora.Api.Services;

/// <summary>
/// Carga datos de ejemplo en la base de demostración para poder probar la
/// interfaz sin crear alumnos y materias a mano. Solo se ejecuta en el
/// entorno "Demo" y no duplica datos si ya existen.
/// </summary>
public static class DemoDataSeeder
{
    private static readonly Guid ProfesoraId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static readonly Guid MatematicaId = Guid.Parse("33333333-0000-0000-0000-000000000001");
    private static readonly Guid InglesId = Guid.Parse("33333333-0000-0000-0000-000000000002");
    private static readonly Guid FisicaId = Guid.Parse("33333333-0000-0000-0000-000000000003");
    private static readonly Guid HistoriaId = Guid.Parse("33333333-0000-0000-0000-000000000004");

    private static readonly Guid LuciaId = Guid.Parse("22222222-0000-0000-0000-000000000001");
    private static readonly Guid TomasId = Guid.Parse("22222222-0000-0000-0000-000000000002");
    private static readonly Guid MartinaId = Guid.Parse("22222222-0000-0000-0000-000000000003");
    private static readonly Guid JoaquinId = Guid.Parse("22222222-0000-0000-0000-000000000004");

    public static async Task SeedAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GestionProfesoraDbContext>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(DemoDataSeeder));

        await context.Database.MigrateAsync(cancellationToken);

        var yaHayAlumnos = await context.Alumnos
            .AnyAsync(a => a.IdProfesora == ProfesoraId, cancellationToken);

        if (yaHayAlumnos)
        {
            logger.LogInformation("La base de demostración ya tiene datos; no se vuelve a sembrar.");
            return;
        }

        var alumnos = new[]
        {
            new Alumno
            {
                Id = LuciaId,
                IdProfesora = ProfesoraId,
                Nombre = "Lucía",
                Apellido = "Fernández",
                Email = "lucia.fernandez@ejemplo.com",
                Telefono = "11 5555-1001",
                NivelEducativo = "Secundaria",
                Observaciones = "Necesita refuerzo en fracciones y ecuaciones.",
                Activo = true,
                FechaAlta = new DateTime(2026, 2, 3, 0, 0, 0, DateTimeKind.Utc)
            },
            new Alumno
            {
                Id = TomasId,
                IdProfesora = ProfesoraId,
                Nombre = "Tomás",
                Apellido = "Rojas",
                Email = "tomas.rojas@ejemplo.com",
                Telefono = "11 5555-1002",
                NivelEducativo = "Secundaria",
                Observaciones = "Buen avance en matemática; repasar física antes del examen.",
                Activo = true,
                FechaAlta = new DateTime(2026, 2, 10, 0, 0, 0, DateTimeKind.Utc)
            },
            new Alumno
            {
                Id = MartinaId,
                IdProfesora = ProfesoraId,
                Nombre = "Martina",
                Apellido = "López",
                Email = "martina.lopez@ejemplo.com",
                Telefono = "11 5555-1003",
                NivelEducativo = "Primaria",
                Observaciones = "Clases de inglés con foco en comprensión lectora.",
                Activo = true,
                FechaAlta = new DateTime(2026, 3, 5, 0, 0, 0, DateTimeKind.Utc)
            },
            new Alumno
            {
                Id = JoaquinId,
                IdProfesora = ProfesoraId,
                Nombre = "Joaquín",
                Apellido = "Pérez",
                Email = "joaquin.perez@ejemplo.com",
                Telefono = "11 5555-1004",
                NivelEducativo = "Secundaria",
                Observaciones = "Dejó las clases por el momento. Historial conservado.",
                Activo = false,
                FechaAlta = new DateTime(2025, 8, 12, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        var materias = new[]
        {
            new Materia
            {
                Id = MatematicaId,
                IdProfesora = ProfesoraId,
                Nombre = "Matemática",
                Descripcion = "Álgebra, geometría y resolución de problemas.",
                Activa = true
            },
            new Materia
            {
                Id = InglesId,
                IdProfesora = ProfesoraId,
                Nombre = "Inglés",
                Descripcion = "Comprensión lectora, gramática y conversación.",
                Activa = true
            },
            new Materia
            {
                Id = FisicaId,
                IdProfesora = ProfesoraId,
                Nombre = "Física",
                Descripcion = "Cinemática, dinámica y trabajo práctico.",
                Activa = true
            },
            new Materia
            {
                Id = HistoriaId,
                IdProfesora = ProfesoraId,
                Nombre = "Historia",
                Descripcion = "Historia argentina y latinoamericana.",
                Activa = false
            }
        };

        var asociaciones = new[]
        {
            new AlumnoMateria { IdAlumno = LuciaId, IdMateria = MatematicaId },
            new AlumnoMateria { IdAlumno = LuciaId, IdMateria = InglesId },
            new AlumnoMateria { IdAlumno = TomasId, IdMateria = MatematicaId },
            new AlumnoMateria { IdAlumno = TomasId, IdMateria = FisicaId },
            new AlumnoMateria { IdAlumno = MartinaId, IdMateria = InglesId },
            new AlumnoMateria { IdAlumno = JoaquinId, IdMateria = HistoriaId }
        };

        context.Alumnos.AddRange(alumnos);
        context.Materias.AddRange(materias);
        context.AlumnosMaterias.AddRange(asociaciones);

        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Base de demostración sembrada: {Alumnos} alumnos, {Materias} materias.",
            alumnos.Length,
            materias.Length);
    }
}
