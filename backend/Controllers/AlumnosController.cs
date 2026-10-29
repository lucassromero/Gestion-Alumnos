using GestionProfesora.Api.Dtos;
using GestionProfesora.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionProfesora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlumnosController : ControllerBase
{
    private readonly IAlumnoService _alumnoService;
    private readonly IMateriaService _materiaService;

    public AlumnosController(IAlumnoService alumnoService, IMateriaService materiaService)
    {
        _alumnoService = alumnoService;
        _materiaService = materiaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlumnoResponseDto>>> Listar(
        [FromQuery] bool? activo,
        [FromQuery] string? buscar,
        CancellationToken cancellationToken)
    {
        var alumnos = await _alumnoService.ListarAsync(activo, buscar, cancellationToken);
        return Ok(alumnos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AlumnoResponseDto>> Obtener(Guid id, CancellationToken cancellationToken)
    {
        var alumno = await _alumnoService.ObtenerPorIdAsync(id, cancellationToken);

        if (alumno == null)
        {
            return Problem(
                title: "Alumno no encontrado",
                detail: $"No existe un alumno con el identificador '{id}'.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(alumno);
    }

    [HttpGet("{id:guid}/materias")]
    public async Task<ActionResult<IReadOnlyList<MateriaResponseDto>>> ListarMaterias(
        Guid id,
        CancellationToken cancellationToken)
    {
        var materias = await _materiaService.ListarPorAlumnoAsync(id, cancellationToken);

        if (materias == null)
        {
            return Problem(
                title: "Alumno no encontrado",
                detail: $"No existe un alumno con el identificador '{id}'.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(materias);
    }

    [HttpPost]
    public async Task<ActionResult<AlumnoResponseDto>> Crear(
        [FromBody] AlumnoCreateDto dto,
        CancellationToken cancellationToken)
    {
        var alumno = await _alumnoService.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(
            nameof(Obtener),
            new { id = alumno.Id },
            alumno);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AlumnoResponseDto>> Actualizar(
        Guid id,
        [FromBody] AlumnoUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var alumno = await _alumnoService.ActualizarAsync(id, dto, cancellationToken);

        if (alumno == null)
        {
            return Problem(
                title: "Alumno no encontrado",
                detail: $"No existe un alumno con el identificador '{id}'.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(alumno);
    }

    [HttpPatch("{id:guid}/estado")]
    public async Task<ActionResult<AlumnoResponseDto>> CambiarEstado(
        Guid id,
        [FromBody] CambiarEstadoDto dto,
        CancellationToken cancellationToken)
    {
        var alumno = await _alumnoService.ObtenerPorIdAsync(id, cancellationToken);

        if (alumno == null)
        {
            return Problem(
                title: "Alumno no encontrado",
                detail: $"No existe un alumno con el identificador '{id}'.",
                statusCode: StatusCodes.Status404NotFound);
        }

        await _alumnoService.CambiarEstadoAsync(id, dto.Activo, cancellationToken);

        var actualizado = await _alumnoService.ObtenerPorIdAsync(id, cancellationToken);
        return Ok(actualizado);
    }
}
