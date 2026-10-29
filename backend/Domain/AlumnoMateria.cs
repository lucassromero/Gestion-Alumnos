namespace GestionProfesora.Api.Domain;

public class AlumnoMateria
{
    public Guid IdAlumno { get; set; }

    public Guid IdMateria { get; set; }

    public Alumno Alumno { get; set; } = null!;

    public Materia Materia { get; set; } = null!;
}
