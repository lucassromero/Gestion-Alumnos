using System.Net;
using System.Net.Http.Json;
using GestionProfesora.Api.Dtos;

namespace GestionProfesora.Api.Tests;

public class MateriasApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public MateriasApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task ResetAsync() => await _factory.ResetAsync();

    [Fact]
    public async Task Listar_Inicialmente_DevuelveListaVacia()
    {
        await ResetAsync();

        var response = await _client.GetAsync("/api/materias");

        response.EnsureSuccessStatusCode();
        var materias = await response.Content.ReadFromJsonAsync<List<MateriaResponseDto>>();

        Assert.NotNull(materias);
        Assert.Empty(materias);
    }

    [Fact]
    public async Task Crear_Valida_DevuelveCreatedYApareceEnListado()
    {
        await ResetAsync();

        var dto = new MateriaCreateDto
        {
            Nombre = "Matemática",
            Descripcion = "Álgebra y geometría"
        };

        var response = await _client.PostAsJsonAsync("/api/materias", dto);
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var creada = await response.Content.ReadFromJsonAsync<MateriaResponseDto>();
        Assert.NotNull(creada);
        Assert.Equal("Matemática", creada.Nombre);
        Assert.True(creada.Activa);

        var listResponse = await _client.GetAsync("/api/materias");
        listResponse.EnsureSuccessStatusCode();
        var materias = await listResponse.Content.ReadFromJsonAsync<List<MateriaResponseDto>>();

        Assert.Single(materias!);
        Assert.Equal(creada.Id, materias![0].Id);
    }

    [Fact]
    public async Task Crear_NombreDuplicado_DevuelveConflict()
    {
        await ResetAsync();

        var dto = new MateriaCreateDto { Nombre = "Inglés" };

        var primera = await _client.PostAsJsonAsync("/api/materias", dto);
        primera.EnsureSuccessStatusCode();

        var segunda = await _client.PostAsJsonAsync("/api/materias", dto);

        Assert.Equal(HttpStatusCode.Conflict, segunda.StatusCode);
    }

    [Fact]
    public async Task Crear_SinNombre_DevuelveBadRequest()
    {
        await ResetAsync();

        var response = await _client.PostAsJsonAsync("/api/materias", new MateriaCreateDto { Nombre = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Actualizar_Existente_ModificaLosDatos()
    {
        await ResetAsync();
        var creada = await CrearMateriaAsync("Física");

        var response = await _client.PutAsJsonAsync(
            $"/api/materias/{creada.Id}",
            new MateriaUpdateDto
            {
                Nombre = "Física y Química",
                Descripcion = "Ciencias naturales"
            });

        response.EnsureSuccessStatusCode();
        var actualizada = await response.Content.ReadFromJsonAsync<MateriaResponseDto>();

        Assert.NotNull(actualizada);
        Assert.Equal("Física y Química", actualizada.Nombre);
        Assert.Equal("Ciencias naturales", actualizada.Descripcion);
    }

    [Fact]
    public async Task CambiarEstado_DesactivarYReactivar_Persiste()
    {
        await ResetAsync();
        var creada = await CrearMateriaAsync("Historia");

        var desactivar = await _client.PatchAsJsonAsync(
            $"/api/materias/{creada.Id}/estado",
            new CambiarEstadoMateriaDto { Activa = false });
        desactivar.EnsureSuccessStatusCode();

        var desactivada = await desactivar.Content.ReadFromJsonAsync<MateriaResponseDto>();
        Assert.NotNull(desactivada);
        Assert.False(desactivada.Activa);

        var reactivar = await _client.PatchAsJsonAsync(
            $"/api/materias/{creada.Id}/estado",
            new CambiarEstadoMateriaDto { Activa = true });
        reactivar.EnsureSuccessStatusCode();

        var reactivada = await reactivar.Content.ReadFromJsonAsync<MateriaResponseDto>();
        Assert.NotNull(reactivada);
        Assert.True(reactivada.Activa);
    }

    [Fact]
    public async Task AsociarAlumno_DevuelveNoContentYApareceEnAmbasFichas()
    {
        await ResetAsync();
        var alumno = await CrearAlumnoAsync("Lucía", "Fernández");
        var materia = await CrearMateriaAsync("Lengua");

        var asociar = await _client.PostAsync(
            $"/api/materias/{materia.Id}/alumnos/{alumno.Id}",
            null);
        Assert.Equal(HttpStatusCode.NoContent, asociar.StatusCode);

        var materiasDelAlumno = await _client.GetAsync($"/api/alumnos/{alumno.Id}/materias");
        materiasDelAlumno.EnsureSuccessStatusCode();
        var lista = await materiasDelAlumno.Content.ReadFromJsonAsync<List<MateriaResponseDto>>();
        Assert.Single(lista!);
        Assert.Equal(materia.Id, lista![0].Id);

        var detalle = await _client.GetAsync($"/api/materias/{materia.Id}");
        detalle.EnsureSuccessStatusCode();
        var detalleMateria = await detalle.Content.ReadFromJsonAsync<MateriaDetalleDto>();
        Assert.NotNull(detalleMateria);
        Assert.Single(detalleMateria.Alumnos);
        Assert.Equal(alumno.Id, detalleMateria.Alumnos[0].Id);
    }

    [Fact]
    public async Task DesasociarAlumno_DevuelveNoContentYQuitaLaMateria()
    {
        await ResetAsync();
        var alumno = await CrearAlumnoAsync("Tomás", "Rojas");
        var materia = await CrearMateriaAsync("Biología");

        await _client.PostAsync($"/api/materias/{materia.Id}/alumnos/{alumno.Id}", null);

        var desasociar = await _client.DeleteAsync($"/api/materias/{materia.Id}/alumnos/{alumno.Id}");
        Assert.Equal(HttpStatusCode.NoContent, desasociar.StatusCode);

        var materiasDelAlumno = await _client.GetAsync($"/api/alumnos/{alumno.Id}/materias");
        materiasDelAlumno.EnsureSuccessStatusCode();
        var lista = await materiasDelAlumno.Content.ReadFromJsonAsync<List<MateriaResponseDto>>();
        Assert.Empty(lista!);
    }

    [Fact]
    public async Task Asociar_MateriaInexistente_DevuelveNotFound()
    {
        await ResetAsync();
        var alumno = await CrearAlumnoAsync("Sofía", "Méndez");

        var response = await _client.PostAsync(
            $"/api/materias/{Guid.NewGuid()}/alumnos/{alumno.Id}",
            null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListarMaterias_AlumnoInexistente_DevuelveNotFound()
    {
        await ResetAsync();

        var response = await _client.GetAsync($"/api/alumnos/{Guid.NewGuid()}/materias");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private async Task<MateriaResponseDto> CrearMateriaAsync(string nombre)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/materias",
            new MateriaCreateDto { Nombre = nombre });
        response.EnsureSuccessStatusCode();

        var materia = await response.Content.ReadFromJsonAsync<MateriaResponseDto>();
        Assert.NotNull(materia);
        return materia;
    }

    private async Task<AlumnoResponseDto> CrearAlumnoAsync(string nombre, string apellido)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/alumnos",
            new AlumnoCreateDto { Nombre = nombre, Apellido = apellido });
        response.EnsureSuccessStatusCode();

        var alumno = await response.Content.ReadFromJsonAsync<AlumnoResponseDto>();
        Assert.NotNull(alumno);
        return alumno;
    }
}
