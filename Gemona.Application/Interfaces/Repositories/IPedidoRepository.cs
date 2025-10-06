using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Application.Interfaces.Repositories
{
    public interface IPedidoRepository : IBaseRepository<Pedido>
    {
        Task<IEnumerable<Pedido>> GetPedidosByClienteAsync(int clienteId);
        Task<IEnumerable<Pedido>> GetPedidosByEstabelecimentoAsync(int estabelecimentoId);
        Task<IEnumerable<Pedido>> GetPedidosByStatusAsync(StatusPedido status);
        Task<Pedido?> GetPedidoCompletoAsync(int pedidoId);
        Task<IEnumerable<Pedido>> GetPedidosPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<decimal> GetTotalVendasEstabelecimentoAsync(int estabelecimentoId, DateTime dataInicio, DateTime dataFim);
    }
}