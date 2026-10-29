namespace GestionProfesora.Api.Services;

public class CurrentProfesoraService : ICurrentProfesora
{
    // Identificador fijo de la profesora única del MVP.
    // Se reemplazará por el usuario autenticado cuando se incorpore autenticación.
    public Guid Id => Guid.Parse("11111111-1111-1111-1111-111111111111");
}
