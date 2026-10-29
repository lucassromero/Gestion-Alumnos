using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using GestionProfesora.Api.Dtos;

namespace GestionProfesora.Api.Tests;

public class AlumnosApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AlumnosApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private readonly CustomWebApplicationFactory _factory;

    private async Task ResetAsync() => await _factory.ResetAsync();

    [Fact]
    public async Task Listar_Inicialmente_DevuelveListaVacia()
    {
        await ResetAsync();
        var response = await _client.GetAsync("/api/alumnos");

        response.EnsureSuccessStatusCode();
        var alumnos = await response.Content.ReadFromJsonAsync<List<AlumnoResponseDto>>();

        Assert.NotNull(alumnos);
        Assert.Empty(alumnos);
    }

    [Fact]
    public async Task Crear_Valido_DevuelveCreatedYApareceEnListado()
    {
        await ResetAsync();
        var dto = new AlumnoCreateDto
        {
            Nombre = "Ana",
            Apellido = "García",
            Email = "ana.garcia@ejemplo.com",
            Telefono = "123456789",
            NivelEducativo = "Secundaria",
            Observaciones = "Nueva alumna"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/alumnos", dto);
        createResponse.EnsureSuccessStatusCode();

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var creado = await createResponse.Content.ReadFromJsonAsync<AlumnoResponseDto>();
        Assert.NotNull(creado);
        Assert.Equal("Ana", creado.Nombre);
        Assert.Equal("García", creado.Apellido);
        Assert.True(creado.Activo);

        var listResponse = await _client.GetAsync("/api/alumnos");
        listResponse.EnsureSuccessStatusCode();
        var alumnos = await listResponse.Content.ReadFromJsonAsync<List<AlumnoResponseDto>>();

        Assert.Single(alumnos!);
        Assert.Equal(creado.Id, alumnos![0].Id);
    }

    [Fact]
    public async Task Crear_Incompleto_DevuelveBadRequest()
    {
        await ResetAsync();
        var dto = new AlumnoCreateDto
        {
            Nombre = "",
            Apellido = ""
        };

        var response = await _client.PostAsJsonAsync("/api/alumnos", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Crear_EmailInvalido_DevuelveBadRequest()
    {
        await ResetAsync();
        var dto = new AlumnoCreateDto
        {
            Nombre = "Luis",
            Apellido = "Pérez",
            Email = "no-es-un-email"
        };

        var response = await _client.PostAsJsonAsync("/api/alumnos", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Obtener_NoExistente_DevuelveNotFound()
    {
        await ResetAsync();
        var response = await _client.GetAsync($"/api/alumnos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Actualizar_Existente_ModificaLosDatos()
    {
        await ResetAsync();
        var creado = await CrearAlumnoAsync("Carlos", "López");

        var updateDto = new AlumnoUpdateDto
        {
            Nombre = "Carlos",
            Apellido = "López Martínez",
            Email = "carlos.lopez@ejemplo.com",
            Telefono = "987654321",
            NivelEducativo = "Universitario",
            Observaciones = "Actualizado"
        };

        var response = await _client.PutAsJsonAsync($"/api/alumnos/{creado.Id}", updateDto);
        response.EnsureSuccessStatusCode();

        var actualizado = await response.Content.ReadFromJsonAsync<AlumnoResponseDto>();
        Assert.NotNull(actualizado);
        Assert.Equal("López Martínez", actualizado.Apellido);
        Assert.Equal("carlos.lopez@ejemplo.com", actualizado.Email);
    }

    [Fact]
    public async Task CambiarEstado_DesactivarYReactivar_PersisteHistorial()
    {
        await ResetAsync();
        var creado = await CrearAlumnoAsync("María", "Sosa");

        var deactivateResponse = await _client.PatchAsJsonAsync(
            $"/api/alumnos/{creado.Id}/estado",
            new CambiarEstadoDto { Activo = false });
        deactivateResponse.EnsureSuccessStatusCode();

        var desactivado = await deactivateResponse.Content.ReadFromJsonAsync<AlumnoResponseDto>();
        Assert.NotNull(desactivado);
        Assert.False(desactivado.Activo);

        var inactivos = await _client.GetAsync("/api/alumnos?activo=false");
        inactivos.EnsureSuccessStatusCode();
        var listaInactivos = await inactivos.Content.ReadFromJsonAsync<List<AlumnoResponseDto>>();
        Assert.Single(listaInactivos!);

        var activateResponse = await _client.PatchAsJsonAsync(
            $"/api/alumnos/{creado.Id}/estado",
            new CambiarEstadoDto { Activo = true });
        activateResponse.EnsureSuccessStatusCode();

        var reactivado = await activateResponse.Content.ReadFromJsonAsync<AlumnoResponseDto>();
        Assert.NotNull(reactivado);
        Assert.True(reactivado.Activo);
        Assert.Equal(creado.Id, reactivado.Id);
    }

    [Fact]
    public async Task Listar_ConBusqueda_FiltraPorNombreApellidoYEmail()
    {
        await ResetAsync();
        await CrearAlumnoAsync("Juan", "Pérez", "juan.perez@ejemplo.com");
        await CrearAlumnoAsync("Laura", "Gómez", "laura.gomez@ejemplo.com");

        var response = await _client.GetAsync("/api/alumnos?buscar=perez");
        response.EnsureSuccessStatusCode();
        var alumnos = await response.Content.ReadFromJsonAsync<List<AlumnoResponseDto>>();

        Assert.Single(alumnos!);
        Assert.Equal("Pérez", alumnos![0].Apellido);
    }

    private async Task<AlumnoResponseDto> CrearAlumnoAsync(
        string nombre,
        string apellido,
        string? email = null)
    {
        var dto = new AlumnoCreateDto
        {
            Nombre = nombre,
            Apellido = apellido,
            Email = email
        };

        var response = await _client.PostAsJsonAsync("/api/alumnos", dto);
        response.EnsureSuccessStatusCode();

        var alumno = await response.Content.ReadFromJsonAsync<AlumnoResponseDto>();
        Assert.NotNull(alumno);
        return alumno;
    }
}
