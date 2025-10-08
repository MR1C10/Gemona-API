using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class AvaliacaoRepository : BaseRepository<Avaliacao>, IAvaliacaoRepository
    {
        public AvaliacaoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Avaliacao>> GetAvaliacoesByClienteAsync(int clienteId)
        {
            return await _dbSet
                .Include(a => a.Pedido)
                .ThenInclude(p => p.Servico)
                .Where(a => a.ClienteId == clienteId)
                .OrderByDescending(a => a.Data)
                .ToListAsync();
        }

        public async Task<IEnumerable<Avaliacao>> GetAvaliacoesByEstabelecimentoAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Include(a => a.Cliente)
                .Include(a => a.Pedido)
                .ThenInclude(p => p.Servico)
                .Where(a => a.Pedido.Servico.EstabelecimentoId == estabelecimentoId)
                .OrderByDescending(a => a.Data)
                .ToListAsync();
        }

        public async Task<Avaliacao?> GetAvaliacaoByPedidoAsync(int pedidoId)
        {
            return await _dbSet
                .Include(a => a.Cliente)
                .FirstOrDefaultAsync(a => a.PedidoId == pedidoId);
        }

        public async Task<double> GetMediaAvaliacoesEstabelecimentoAsync(int estabelecimentoId)
        {
            var avaliacoes = await _dbSet
                .Include(a => a.Pedido)
                .ThenInclude(p => p.Servico)
                .Where(a => a.Pedido.Servico.EstabelecimentoId == estabelecimentoId)
                .Select(a => (int)a.Nota)
                .ToListAsync();

            return avaliacoes.Any() ? avaliacoes.Average() : 0;
        }

        public async Task<IEnumerable<Avaliacao>> GetAvaliacoesByNotaAsync(NotaAvaliacao nota)
        {
            return await _dbSet
                .Include(a => a.Cliente)
                .Include(a => a.Pedido)
                .Where(a => a.Nota == nota)
                .ToListAsync();
        }

        public async Task<bool> ClienteJaAvaliouPedidoAsync(int clienteId, int pedidoId)
        {
            return await _dbSet
                .AnyAsync(a => a.ClienteId == clienteId && a.PedidoId == pedidoId);
        }
    }
}