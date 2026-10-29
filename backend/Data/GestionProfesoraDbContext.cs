using GestionProfesora.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestionProfesora.Api.Data;

public class GestionProfesoraDbContext : DbContext
{
    public GestionProfesoraDbContext(DbContextOptions<GestionProfesoraDbContext> options)
        : base(options)
    {
    }

    public DbSet<Profesora> Profesoras => Set<Profesora>();

    public DbSet<Alumno> Alumnos => Set<Alumno>();

    public DbSet<Materia> Materias => Set<Materia>();

    public DbSet<AlumnoMateria> AlumnosMaterias => Set<AlumnoMateria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Profesora>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(p => p.Email)
                .HasMaxLength(150);

            entity.HasIndex(p => p.Email)
                .IsUnique()
                .HasFilter("[Email] IS NOT NULL");
        });

        modelBuilder.Entity<Alumno>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(a => a.Apellido)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(a => a.Email)
                .HasMaxLength(150);

            entity.Property(a => a.Telefono)
                .HasMaxLength(50);

            entity.Property(a => a.NivelEducativo)
                .HasMaxLength(100);

            entity.Property(a => a.Observaciones)
                .HasMaxLength(1000);

            entity.HasOne(a => a.Profesora)
                .WithMany(p => p.Alumnos)
                .HasForeignKey(a => a.IdProfesora)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => new { a.IdProfesora, a.Activo });
        });

        modelBuilder.Entity<Materia>(entity =>
        {
            entity.HasKey(m => m.Id);

            entity.Property(m => m.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(m => m.Descripcion)
                .HasMaxLength(500);

            entity.HasOne(m => m.Profesora)
                .WithMany(p => p.Materias)
                .HasForeignKey(m => m.IdProfesora)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(m => new { m.IdProfesora, m.Nombre })
                .IsUnique();
        });

        modelBuilder.Entity<AlumnoMateria>(entity =>
        {
            entity.HasKey(am => new { am.IdAlumno, am.IdMateria });

            entity.HasOne(am => am.Alumno)
                .WithMany(a => a.AlumnosMaterias)
                .HasForeignKey(am => am.IdAlumno)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(am => am.Materia)
                .WithMany(m => m.AlumnosMaterias)
                .HasForeignKey(am => am.IdMateria)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Semilla de la profesora única del MVP.
        // Se usa un GUID determinista para que las migraciones sean reproducibles.
        modelBuilder.Entity<Profesora>().HasData(new Profesora
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Nombre = "Profesora Demo",
            Email = "profesora@ejemplo.com",
            FechaAlta = new DateTime(2026, 10, 29, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
