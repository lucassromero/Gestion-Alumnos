using GestionProfesora.Api.Data;
using GestionProfesora.Api.Domain;
using GestionProfesora.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GestionProfesora.Api.Services;

public class MateriaService : IMateriaService
{
    private readonly GestionProfesoraDbContext _context;
    private readonly ICurrentProfesora _currentProfesora;

    public MateriaService(GestionProfesoraDbContext context, ICurrentProfesora currentProfesora)
    {
        _context = context;
        _currentProfesora = currentProfesora;
    }

    public async Task<IEnumerable<MateriaResponseDto>> ListarAsync(
        bool? activa,
        string? buscar,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Materias
            .AsNoTracking()
            .Where(m => m.IdProfesora == _currentProfesora.Id);

        if (activa.HasValue)
        {
            query = query.Where(m => m.Activa == activa.Value);
        }

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var filtro = buscar.Trim().ToLower();
            query = query.Where(m =>
                m.Nombre.ToLower().Contains(filtro) ||
                (m.Descripcion != null && m.Descripcion.ToLower().Contains(filtro)));
        }

        return await query
            .OrderBy(m => m.Nombre)
            .Select(m => new MateriaResponseDto
            {
                Id = m.Id,
                IdProfesora = m.IdProfesora,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                Activa = m.Activa,
                CantidadAlumnos = m.AlumnosMaterias.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<MateriaDetalleDto?> ObtenerPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var materia = await _context.Materias
            .AsNoTracking()
            .Include(m => m.AlumnosMaterias)
            .ThenInclude(am => am.Alumno)
            .FirstOrDefaultAsync(m => m.Id == id && m.IdProfesora == _currentProfesora.Id, cancellationToken);

        if (materia == null)
        {
            return null;
        }

        return new MateriaDetalleDto
        {
            Id = materia.Id,
            IdProfesora = materia.IdProfesora,
            Nombre = materia.Nombre,
            Descripcion = materia.Descripcion,
            Activa = materia.Activa,
            Alumnos = materia.AlumnosMaterias
                .Select(am => new AlumnoResumenDto
                {
                    Id = am.Alumno.Id,
                    Nombre = am.Alumno.Nombre,
                    Apellido = am.Alumno.Apellido,
                    Activo = am.Alumno.Activo
                })
                .OrderBy(a => a.Apellido)
                .ThenBy(a => a.Nombre)
                .ToList()
        };
    }

    public async Task<MateriaResponseDto> CrearAsync(MateriaCreateDto dto, CancellationToken cancellationToken = default)
    {
        var materia = new Materia
        {
            Id = Guid.NewGuid(),
            IdProfesora = _currentProfesora.Id,
            Nombre = dto.Nombre.Trim(),
            Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim(),
            Activa = true
        };

        _context.Materias.Add(materia);
        await _context.SaveChangesAsync(cancellationToken);

        return new MateriaResponseDto
        {
            Id = materia.Id,
            IdProfesora = materia.IdProfesora,
            Nombre = materia.Nombre,
            Descripcion = materia.Descripcion,
            Activa = materia.Activa,
            CantidadAlumnos = 0
        };
    }

    public async Task<MateriaResponseDto?> ActualizarAsync(
        Guid id,
        MateriaUpdateDto dto,
        CancellationToken cancellationToken = default)
    {
        var materia = await _context.Materias
            .FirstOrDefaultAsync(m => m.Id == id && m.IdProfesora == _currentProfesora.Id, cancellationToken);

        if (materia == null)
        {
            return null;
        }

        materia.Nombre = dto.Nombre.Trim();
        materia.Descripcion = string.IsNullOrWhiteSpace(dto.Descripcion) ? null : dto.Descripcion.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        return await MapearConCantidadAsync(materia, cancellationToken);
    }

    public async Task<MateriaResponseDto?> CambiarEstadoAsync(
        Guid id,
        bool activa,
        CancellationToken cancellationToken = default)
    {
        var materia = await _context.Materias
            .FirstOrDefaultAsync(m => m.Id == id && m.IdProfesora == _currentProfesora.Id, cancellationToken);

        if (materia == null)
        {
            return null;
        }

        materia.Activa = activa;
        await _context.SaveChangesAsync(cancellationToken);

        return await MapearConCantidadAsync(materia, cancellationToken);
    }

    public async Task<bool> ExisteNombreAsync(
        string nombre,
        Guid? excluirId = null,
        CancellationToken cancellationToken = default)
    {
        var nombreNormalizado = nombre.Trim();
        var query = _context.Materias
            .AsNoTracking()
            .Where(m => m.IdProfesora == _currentProfesora.Id && m.Nombre == nombreNormalizado);

        if (excluirId.HasValue)
        {
            query = query.Where(m => m.Id != excluirId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> AsociarAlumnoAsync(
        Guid idMateria,
        Guid idAlumno,
        CancellationToken cancellationToken = default)
    {
        var materiaExiste = await _context.Materias
            .AnyAsync(m => m.Id == idMateria && m.IdProfesora == _currentProfesora.Id, cancellationToken);

        var alumnoExiste = await _context.Alumnos
            .AnyAsync(a => a.Id == idAlumno && a.IdProfesora == _currentProfesora.Id, cancellationToken);

        if (!materiaExiste || !alumnoExiste)
        {
            return false;
        }

        var yaAsociada = await _context.AlumnosMaterias
            .AnyAsync(am => am.IdMateria == idMateria && am.IdAlumno == idAlumno, cancellationToken);

        if (yaAsociada)
        {
            return true;
        }

        _context.AlumnosMaterias.Add(new AlumnoMateria
        {
            IdMateria = idMateria,
            IdAlumno = idAlumno
        });

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DesasociarAlumnoAsync(
        Guid idMateria,
        Guid idAlumno,
        CancellationToken cancellationToken = default)
    {
        var materiaExiste = await _context.Materias
            .AnyAsync(m => m.Id == idMateria && m.IdProfesora == _currentProfesora.Id, cancellationToken);

        if (!materiaExiste)
        {
            return false;
        }

        var relacion = await _context.AlumnosMaterias
            .FirstOrDefaultAsync(am => am.IdMateria == idMateria && am.IdAlumno == idAlumno, cancellationToken);

        if (relacion == null)
        {
            return false;
        }

        _context.AlumnosMaterias.Remove(relacion);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<MateriaResponseDto>?> ListarPorAlumnoAsync(
        Guid idAlumno,
        CancellationToken cancellationToken = default)
    {
        var alumnoExiste = await _context.Alumnos
            .AnyAsync(a => a.Id == idAlumno && a.IdProfesora == _currentProfesora.Id, cancellationToken);

        if (!alumnoExiste)
        {
            return null;
        }

        return await _context.Materias
            .AsNoTracking()
            .Where(m => m.IdProfesora == _currentProfesora.Id &&
                        m.AlumnosMaterias.Any(am => am.IdAlumno == idAlumno))
            .OrderBy(m => m.Nombre)
            .Select(m => new MateriaResponseDto
            {
                Id = m.Id,
                IdProfesora = m.IdProfesora,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                Activa = m.Activa,
                CantidadAlumnos = m.AlumnosMaterias.Count
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<MateriaResponseDto> MapearConCantidadAsync(
        Materia materia,
        CancellationToken cancellationToken)
    {
        var cantidadAlumnos = await _context.AlumnosMaterias
            .CountAsync(am => am.IdMateria == materia.Id, cancellationToken);

        return new MateriaResponseDto
        {
            Id = materia.Id,
            IdProfesora = materia.IdProfesora,
            Nombre = materia.Nombre,
            Descripcion = materia.Descripcion,
            Activa = materia.Activa,
            CantidadAlumnos = cantidadAlumnos
        };
    }
}
