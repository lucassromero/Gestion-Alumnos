using GestionProfesora.Api.Dtos;

namespace GestionProfesora.Api.Services;

public interface IAlumnoService
{
    Task<IEnumerable<AlumnoResponseDto>> ListarAsync(bool? activo, string? buscar, CancellationToken cancellationToken = default);
    Task<AlumnoResponseDto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<AlumnoResponseDto> CrearAsync(AlumnoCreateDto dto, CancellationToken cancellationToken = default);
    Task<AlumnoResponseDto?> ActualizarAsync(Guid id, AlumnoUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> CambiarEstadoAsync(Guid id, bool activo, CancellationToken cancellationToken = default);
}
