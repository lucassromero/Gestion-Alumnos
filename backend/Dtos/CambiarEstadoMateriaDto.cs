using System.ComponentModel.DataAnnotations;

namespace GestionProfesora.Api.Dtos;

public class CambiarEstadoMateriaDto
{
    [Required]
    public bool Activa { get; set; }
}
