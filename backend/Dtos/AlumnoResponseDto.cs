namespace GestionProfesora.Api.Dtos;

public class AlumnoResponseDto
{
    public Guid Id { get; set; }

    public Guid IdProfesora { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Telefono { get; set; }

    public string? NivelEducativo { get; set; }

    public string? Observaciones { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaAlta { get; set; }
}
