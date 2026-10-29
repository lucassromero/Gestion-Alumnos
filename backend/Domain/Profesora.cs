namespace GestionProfesora.Api.Domain;

public class Profesora
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Email { get; set; }

    public DateTime FechaAlta { get; set; }

    public ICollection<Alumno> Alumnos { get; set; } = [];

    public ICollection<Materia> Materias { get; set; } = [];
}
