using System.ComponentModel.DataAnnotations;

namespace GestionProfesora.Api.Dtos;

public class AlumnoCreateDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El apellido no puede superar los 100 caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [MaxLength(150, ErrorMessage = "El email no puede superar los 150 caracteres.")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
    public string? Email { get; set; }

    [MaxLength(50, ErrorMessage = "El teléfono no puede superar los 50 caracteres.")]
    public string? Telefono { get; set; }

    [MaxLength(100, ErrorMessage = "El nivel educativo no puede superar los 100 caracteres.")]
    public string? NivelEducativo { get; set; }

    [MaxLength(1000, ErrorMessage = "Las observaciones no pueden superar los 1000 caracteres.")]
    public string? Observaciones { get; set; }
}
