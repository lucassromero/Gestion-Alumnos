namespace GestionProfesora.Api.Dtos;

public class MateriaDetalleDto
{
    public Guid Id { get; set; }

    public Guid IdProfesora { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public bool Activa { get; set; }

    public List<AlumnoResumenDto> Alumnos { get; set; } = [];
}
