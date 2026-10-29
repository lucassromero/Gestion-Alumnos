namespace GestionProfesora.Api.Domain;

public class Materia
{
    public Guid Id { get; set; }

    public Guid IdProfesora { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activa { get; set; }

    public Profesora Profesora { get; set; } = null!;

    public ICollection<AlumnoMateria> AlumnosMaterias { get; set; } = [];
}
