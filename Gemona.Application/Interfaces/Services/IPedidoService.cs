using Gemona.Application.DTOs.Request.Pedido;
using Gemona.Application.DTOs.Response.Pedido;
using Gemona.Application.DTOs.Shared;
using Gemona.Domain.Enums;

namespace Gemona.Application.Interfaces.Services
{
    public interface IPedidoService
    {
        Task<ApiResponse<IEnumerable<PedidoResponse>>> GetAllAsync();
        Task<ApiResponse<PedidoResponse?>> GetByIdAsync(int id);
        Task<ApiResponse<PedidoCompletoResponse?>> GetPedidoCompletoAsync(int pedidoId);
        Task<ApiResponse<IEnumerable<PedidoResponse>>> GetPedidosByClienteAsync(int clienteId);
        Task<ApiResponse<IEnumerable<PedidoResponse>>> GetPedidosByEstabelecimentoAsync(int estabelecimentoId);
        Task<ApiResponse<IEnumerable<PedidoResponse>>> GetPedidosByStatusAsync(StatusPedido status);
        Task<ApiResponse<PagedResponse<PedidoResponse>>> GetPedidosPorPeriodoAsync(DateTime dataInicio, DateTime dataFim, int pageNumber = 1, int pageSize = 10);
        Task<ApiResponse<decimal>> GetTotalVendasEstabelecimentoAsync(int estabelecimentoId, DateTime dataInicio, DateTime dataFim);
        Task<ApiResponse<PedidoResponse>> CreateAsync(CreatePedidoRequest request);
        Task<ApiResponse<PedidoResponse>> UpdateStatusAsync(int id, UpdateStatusPedidoRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}