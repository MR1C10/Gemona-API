using Gemona.Application.DTOs.Request.Profissional;
using Gemona.Application.DTOs.Response.Profissional;
using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.Interfaces.Services
{
    public interface IProfissionalService
    {
        Task<ApiResponse<IEnumerable<ProfissionalResponse>>> GetAllAsync();
        Task<ApiResponse<ProfissionalResponse?>> GetByIdAsync(int id);
        Task<ApiResponse<ProfissionalResponse?>> GetByEmailAsync(string email);
        Task<ApiResponse<ProfissionalResponse?>> GetByCpfAsync(string cpf);
        Task<ApiResponse<ProfissionalWithEstabelecimentoResponse?>> GetProfissionalWithEstabelecimentoAsync(int profissionalId);
        Task<ApiResponse<ProfissionalResponse>> CreateAsync(CreateProfissionalRequest request);
        Task<ApiResponse<ProfissionalResponse>> UpdateAsync(int id, UpdateProfissionalRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> EmailExistsAsync(string email);
        Task<ApiResponse<bool>> CpfExistsAsync(string cpf);
    }
}