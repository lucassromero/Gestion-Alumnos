using GestionProfesora.Api.Data;
using GestionProfesora.Api.Domain;
using GestionProfesora.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GestionProfesora.Api.Services;

public class AlumnoService : IAlumnoService
{
    private readonly GestionProfesoraDbContext _context;
    private readonly ICurrentProfesora _currentProfesora;

    public AlumnoService(GestionProfesoraDbContext context, ICurrentProfesora currentProfesora)
    {
        _context = context;
        _currentProfesora = currentProfesora;
    }

    public async Task<IEnumerable<AlumnoResponseDto>> ListarAsync(
        bool? activo,
        string? buscar,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Alumnos
            .AsNoTracking()
            .Where(a => a.IdProfesora == _currentProfesora.Id);

        if (activo.HasValue)
        {
            query = query.Where(a => a.Activo == activo.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var filtro = buscar.Trim().ToLower();
            query = query.Where(a =>
                (a.Nombre + " " + a.Apellido).ToLower().Contains(filtro) ||
                (a.Email != null && a.Email.ToLower().Contains(filtro)));
        }

        var alumnos = await query
            .OrderBy(a => a.Apellido)
            .ThenBy(a => a.Nombre)
            .ToListAsync(cancellationToken);

        return alumnos.Select(Mapear);
    }

    public async Task<AlumnoResponseDto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var alumno = await _context.Alumnos
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.IdProfesora == _currentProfesora.Id, cancellationToken);

        return alumno == null ? null : Mapear(alumno);
    }

    public async Task<AlumnoResponseDto> CrearAsync(AlumnoCreateDto dto, CancellationToken cancellationToken = default)
    {
        var alumno = new Alumno
        {
            Id = Guid.NewGuid(),
            IdProfesora = _currentProfesora.Id,
            Nombre = dto.Nombre.Trim(),
            Apellido = dto.Apellido.Trim(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim().ToLower(),
            Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim(),
            NivelEducativo = string.IsNullOrWhiteSpace(dto.NivelEducativo) ? null : dto.NivelEducativo.Trim(),
            Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones) ? null : dto.Observaciones.Trim(),
            Activo = true,
            FechaAlta = DateTime.UtcNow
        };

        _context.Alumnos.Add(alumno);
        await _context.SaveChangesAsync(cancellationToken);

        return Mapear(alumno);
    }

    public async Task<AlumnoResponseDto?> ActualizarAsync(Guid id, AlumnoUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var alumno = await _context.Alumnos
            .FirstOrDefaultAsync(a => a.Id == id && a.IdProfesora == _currentProfesora.Id, cancellationToken);

        if (alumno == null)
        {
            return null;
        }

        alumno.Nombre = dto.Nombre.Trim();
        alumno.Apellido = dto.Apellido.Trim();
        alumno.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim().ToLower();
        alumno.Telefono = string.IsNullOrWhiteSpace(dto.Telefono) ? null : dto.Telefono.Trim();
        alumno.NivelEducativo = string.IsNullOrWhiteSpace(dto.NivelEducativo) ? null : dto.NivelEducativo.Trim();
        alumno.Observaciones = string.IsNullOrWhiteSpace(dto.Observaciones) ? null : dto.Observaciones.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        return Mapear(alumno);
    }

    public async Task<bool> CambiarEstadoAsync(Guid id, bool activo, CancellationToken cancellationToken = default)
    {
        var alumno = await _context.Alumnos
            .FirstOrDefaultAsync(a => a.Id == id && a.IdProfesora == _currentProfesora.Id, cancellationToken);

        if (alumno == null)
        {
            return false;
        }

        alumno.Activo = activo;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static AlumnoResponseDto Mapear(Alumno alumno) => new()
    {
        Id = alumno.Id,
        IdProfesora = alumno.IdProfesora,
        Nombre = alumno.Nombre,
        Apellido = alumno.Apellido,
        Email = alumno.Email,
        Telefono = alumno.Telefono,
        NivelEducativo = alumno.NivelEducativo,
        Observaciones = alumno.Observaciones,
        Activo = alumno.Activo,
        FechaAlta = alumno.FechaAlta
    };
}
