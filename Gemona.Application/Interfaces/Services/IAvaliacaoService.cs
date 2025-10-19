using Gemona.Application.DTOs.Request.Avaliacao;
using Gemona.Application.DTOs.Response.Avaliacao;
using Gemona.Application.DTOs.Shared;
using Gemona.Domain.Enums;

namespace Gemona.Application.Interfaces.Services
{
    public interface IAvaliacaoService
    {
        Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAllAsync();
        Task<ApiResponse<AvaliacaoResponse?>> GetByIdAsync(int id);
        Task<ApiResponse<AvaliacaoResponse?>> GetAvaliacaoByPedidoAsync(int pedidoId);
        Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAvaliacoesByClienteAsync(int clienteId);
        Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAvaliacoesByEstabelecimentoAsync(int estabelecimentoId);
        Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAvaliacoesByNotaAsync(NotaAvaliacao nota);
        Task<ApiResponse<MediaAvaliacoesResponse>> GetMediaAvaliacoesEstabelecimentoAsync(int estabelecimentoId);
        Task<ApiResponse<PagedResponse<AvaliacaoResponse>>> FiltrarAvaliacoesAsync(FiltrarAvaliacoesRequest request);
        Task<ApiResponse<AvaliacaoResponse>> CreateAsync(CreateAvaliacaoRequest request);
        Task<ApiResponse<AvaliacaoResponse>> UpdateAsync(int id, UpdateAvaliacaoRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> ClienteJaAvaliouPedidoAsync(int clienteId, int pedidoId);
    }
}