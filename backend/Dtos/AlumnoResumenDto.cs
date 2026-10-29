namespace GestionProfesora.Api.Dtos;

public class AlumnoResumenDto
{
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Apellido { get; set; } = string.Empty;

    public bool Activo { get; set; }
}
