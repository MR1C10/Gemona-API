using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Application.Interfaces.Repositories
{
    public interface IPedidoHistoricoRepository : IBaseRepository<PedidoHistorico>
    {
        Task<IEnumerable<PedidoHistorico>> GetHistoricoByPedidoAsync(int pedidoId);
        Task<IEnumerable<PedidoHistorico>> GetHistoricoByStatusAsync(StatusPedido status);
        Task<PedidoHistorico?> GetUltimaAlteracaoPedidoAsync(int pedidoId);
        Task AddHistoricoAsync(int pedidoId, StatusPedido statusAnterior, StatusPedido statusNovo);
    }
}