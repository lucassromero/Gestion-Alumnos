namespace GestionProfesora.Api.Dtos;

public class MateriaResponseDto
{
    public Guid Id { get; set; }

    public Guid IdProfesora { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activa { get; set; }

    public int CantidadAlumnos { get; set; }
}
