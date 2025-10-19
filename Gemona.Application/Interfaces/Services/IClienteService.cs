using Gemona.Application.DTOs.Request.Cliente;
using Gemona.Application.DTOs.Response.Cliente;
using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.Interfaces.Services
{
    public interface IClienteService
    {
        Task<ApiResponse<IEnumerable<ClienteResponse>>> GetAllAsync();
        Task<ApiResponse<ClienteResponse?>> GetByIdAsync(int id);
        Task<ApiResponse<ClienteResponse?>> GetByEmailAsync(string email);
        Task<ApiResponse<ClienteResponse?>> GetByCpfAsync(string cpf);
        Task<ApiResponse<ClienteResponse?>> GetClienteWithEnderecoAsync(int clienteId);
        Task<ApiResponse<IEnumerable<ClienteResponse>>> GetClientesByIdadeAsync(int idadeMinima, int idadeMaxima);
        Task<ApiResponse<ClienteResponse>> CreateAsync(CreateClienteRequest request);
        Task<ApiResponse<ClienteResponse>> UpdateAsync(int id, UpdateClienteRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> EmailExistsAsync(string email);
        Task<ApiResponse<bool>> CpfExistsAsync(string cpf);
    }
}