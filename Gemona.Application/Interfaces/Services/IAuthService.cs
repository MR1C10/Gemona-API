using Gemona.Application.DTOs.Request.Cliente;
using Gemona.Application.DTOs.Request.Profissional;
using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<LoginResponse>> LoginClienteAsync(LoginClienteRequest request);
        Task<ApiResponse<LoginResponse>> LoginProfissionalAsync(LoginProfissionalRequest request);
        Task<ApiResponse<LoginResponse>> LoginAdminAsync(string email, string senha);
        Task<ApiResponse<LoginResponse>> LoginAsync(string email, string senha);
        Task<ApiResponse<bool>> ValidateTokenAsync(string token);
        Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string token);
    }
}