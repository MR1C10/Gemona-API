using Gemona.Application.DTOs.Request.Servico;
using Gemona.Application.DTOs.Response.Servico;
using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.Interfaces.Services
{
    public interface IServicoService
    {
        Task<ApiResponse<IEnumerable<ServicoResponse>>> GetAllAsync();
        Task<ApiResponse<ServicoResponse?>> GetByIdAsync(int id);
        Task<ApiResponse<ServicoCompletoResponse?>> GetServicoCompletoAsync(int servicoId);
        Task<ApiResponse<IEnumerable<ServicoResponse>>> GetServicosByEstabelecimentoAsync(int estabelecimentoId);
        Task<ApiResponse<IEnumerable<ServicoResponse>>> GetServicosByCategoriaAsync(int categoriaId);
        Task<ApiResponse<IEnumerable<ServicoResponse>>> GetServicosBySubCategoriaAsync(int subCategoriaId);
        Task<ApiResponse<IEnumerable<ServicoResponse>>> GetServicosByFaixaPrecoAsync(decimal precoMinimo, decimal precoMaximo);
        Task<ApiResponse<PagedResponse<ServicoResponse>>> BuscarServicosAsync(BuscarServicosRequest request);
        Task<ApiResponse<ServicoResponse>> CreateAsync(CreateServicoRequest request);
        Task<ApiResponse<ServicoResponse>> UpdateAsync(int id, UpdateServicoRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}