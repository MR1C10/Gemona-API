using Gemona.Domain.Entities;

namespace Gemona.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateTokenForCliente(Cliente cliente);
        string GenerateTokenForProfissional(Profissional profissional);
        bool ValidateToken(string token);
        int? GetUserIdFromToken(string token);
        string? GetUserTypeFromToken(string token);
    }
}
