using GestionProfesora.Api.Dtos;
using GestionProfesora.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionProfesora.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MateriasController : ControllerBase
{
    private readonly IMateriaService _materiaService;

    public MateriasController(IMateriaService materiaService)
    {
        _materiaService = materiaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MateriaResponseDto>>> Listar(
        [FromQuery] bool? activa,
        [FromQuery] string? buscar,
        CancellationToken cancellationToken)
    {
        var materias = await _materiaService.ListarAsync(activa, buscar, cancellationToken);
        return Ok(materias);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MateriaDetalleDto>> Obtener(Guid id, CancellationToken cancellationToken)
    {
        var materia = await _materiaService.ObtenerPorIdAsync(id, cancellationToken);

        if (materia == null)
        {
            return Problem(
                title: "Materia no encontrada",
                detail: $"No existe una materia con el identificador '{id}'.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(materia);
    }

    [HttpPost]
    public async Task<ActionResult<MateriaResponseDto>> Crear(
        [FromBody] MateriaCreateDto dto,
        CancellationToken cancellationToken)
    {
        if (await _materiaService.ExisteNombreAsync(dto.Nombre, cancellationToken: cancellationToken))
        {
            return Problem(
                title: "Materia duplicada",
                detail: $"Ya existe una materia con el nombre '{dto.Nombre.Trim()}'.",
                statusCode: StatusCodes.Status409Conflict);
        }

        var materia = await _materiaService.CrearAsync(dto, cancellationToken);
        return CreatedAtAction(
            nameof(Obtener),
            new { id = materia.Id },
            materia);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MateriaResponseDto>> Actualizar(
        Guid id,
        [FromBody] MateriaUpdateDto dto,
        CancellationToken cancellationToken)
    {
        if (await _materiaService.ExisteNombreAsync(dto.Nombre, id, cancellationToken))
        {
            return Problem(
                title: "Materia duplicada",
                detail: $"Ya existe otra materia con el nombre '{dto.Nombre.Trim()}'.",
                statusCode: StatusCodes.Status409Conflict);
        }

        var materia = await _materiaService.ActualizarAsync(id, dto, cancellationToken);

        if (materia == null)
        {
            return Problem(
                title: "Materia no encontrada",
                detail: $"No existe una materia con el identificador '{id}'.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(materia);
    }

    [HttpPatch("{id:guid}/estado")]
    public async Task<ActionResult<MateriaResponseDto>> CambiarEstado(
        Guid id,
        [FromBody] CambiarEstadoMateriaDto dto,
        CancellationToken cancellationToken)
    {
        var materia = await _materiaService.CambiarEstadoAsync(id, dto.Activa, cancellationToken);

        if (materia == null)
        {
            return Problem(
                title: "Materia no encontrada",
                detail: $"No existe una materia con el identificador '{id}'.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(materia);
    }

    [HttpPost("{id:guid}/alumnos/{idAlumno:guid}")]
    public async Task<IActionResult> AsociarAlumno(
        Guid id,
        Guid idAlumno,
        CancellationToken cancellationToken)
    {
        var asociada = await _materiaService.AsociarAlumnoAsync(id, idAlumno, cancellationToken);

        if (!asociada)
        {
            return Problem(
                title: "No se pudo asociar la materia",
                detail: "La materia o el alumno no existen o no pertenecen a la profesora actual.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return NoContent();
    }

    [HttpDelete("{id:guid}/alumnos/{idAlumno:guid}")]
    public async Task<IActionResult> DesasociarAlumno(
        Guid id,
        Guid idAlumno,
        CancellationToken cancellationToken)
    {
        var desasociada = await _materiaService.DesasociarAlumnoAsync(id, idAlumno, cancellationToken);

        if (!desasociada)
        {
            return Problem(
                title: "No se pudo desasociar la materia",
                detail: "La materia, el alumno o la asociación no existen.",
                statusCode: StatusCodes.Status404NotFound);
        }

        return NoContent();
    }
}
