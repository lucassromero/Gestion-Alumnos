using System.ComponentModel.DataAnnotations;

namespace GestionProfesora.Api.Dtos;

public class CambiarEstadoDto
{
    [Required]
    public bool Activo { get; set; }
}
