using Gemona.Application.DTOs.Request.Estabelecimento;
using Gemona.Application.DTOs.Response.Estabelecimento;
using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.Interfaces.Services
{
    public interface IEstabelecimentoService
    {
        Task<ApiResponse<IEnumerable<EstabelecimentoResponse>>> GetAllAsync();
        Task<ApiResponse<EstabelecimentoResponse?>> GetByIdAsync(int id);
        Task<ApiResponse<EstabelecimentoResponse?>> GetByCnpjAsync(string cnpj);
        Task<ApiResponse<EstabelecimentoCompletoResponse?>> GetEstabelecimentoCompletoAsync(int estabelecimentoId);
        Task<ApiResponse<IEnumerable<EstabelecimentoResponse>>> GetEstabelecimentosByProfissionalAsync(int profissionalId);
        Task<ApiResponse<IEnumerable<EstabelecimentoResponse>>> GetEstabelecimentosByCidadeAsync(string cidade);
        Task<ApiResponse<IEnumerable<EstabelecimentoResponse>>> GetEstabelecimentosProximosAsync(decimal latitude, decimal longitude, double raioKm);
        Task<ApiResponse<IEnumerable<EstabelecimentoResponse>>> BuscarEstabelecimentosAsync(string termo);
        Task<ApiResponse<EstabelecimentoResponse>> CreateAsync(CreateEstabelecimentoRequest request);
        Task<ApiResponse<EstabelecimentoResponse>> UpdateAsync(int id, UpdateEstabelecimentoRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> CnpjExistsAsync(string cnpj);
    }
}