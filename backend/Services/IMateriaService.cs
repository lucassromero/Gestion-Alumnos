using GestionProfesora.Api.Dtos;

namespace GestionProfesora.Api.Services;

public interface IMateriaService
{
    Task<IEnumerable<MateriaResponseDto>> ListarAsync(bool? activa, string? buscar, CancellationToken cancellationToken = default);
    Task<MateriaDetalleDto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MateriaResponseDto> CrearAsync(MateriaCreateDto dto, CancellationToken cancellationToken = default);
    Task<MateriaResponseDto?> ActualizarAsync(Guid id, MateriaUpdateDto dto, CancellationToken cancellationToken = default);
    Task<MateriaResponseDto?> CambiarEstadoAsync(Guid id, bool activa, CancellationToken cancellationToken = default);
    Task<bool> ExisteNombreAsync(string nombre, Guid? excluirId = null, CancellationToken cancellationToken = default);
    Task<bool> AsociarAlumnoAsync(Guid idMateria, Guid idAlumno, CancellationToken cancellationToken = default);
    Task<bool> DesasociarAlumnoAsync(Guid idMateria, Guid idAlumno, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MateriaResponseDto>?> ListarPorAlumnoAsync(Guid idAlumno, CancellationToken cancellationToken = default);
}
