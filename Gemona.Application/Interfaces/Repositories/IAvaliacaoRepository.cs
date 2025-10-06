using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Application.Interfaces.Repositories
{
    public interface IAvaliacaoRepository : IBaseRepository<Avaliacao>
    {
        Task<IEnumerable<Avaliacao>> GetAvaliacoesByClienteAsync(int clienteId);
        Task<IEnumerable<Avaliacao>> GetAvaliacoesByEstabelecimentoAsync(int estabelecimentoId);
        Task<Avaliacao?> GetAvaliacaoByPedidoAsync(int pedidoId);
        Task<double> GetMediaAvaliacoesEstabelecimentoAsync(int estabelecimentoId);
        Task<IEnumerable<Avaliacao>> GetAvaliacoesByNotaAsync(NotaAvaliacao nota);
        Task<bool> ClienteJaAvaliouPedidoAsync(int clienteId, int pedidoId);
    }
}