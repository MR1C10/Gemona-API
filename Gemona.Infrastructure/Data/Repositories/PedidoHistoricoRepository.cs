using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class PedidoHistoricoRepository : BaseRepository<PedidoHistorico>, IPedidoHistoricoRepository
    {
        public PedidoHistoricoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PedidoHistorico>> GetHistoricoByPedidoAsync(int pedidoId)
        {
            return await _dbSet
                .Where(h => h.PedidoId == pedidoId && h.Ativo)
                .OrderByDescending(h => h.DataAlteracao)
                .ToListAsync();
        }

        public async Task<IEnumerable<PedidoHistorico>> GetHistoricoByStatusAsync(StatusPedido status)
        {
            return await _dbSet
                .Where(h => h.StatusNovo == status && h.Ativo)
                .OrderByDescending(h => h.DataAlteracao)
                .ToListAsync();
        }

        public async Task<PedidoHistorico?> GetUltimaAlteracaoPedidoAsync(int pedidoId)
        {
            return await _dbSet
                .Where(h => h.PedidoId == pedidoId && h.Ativo)
                .OrderByDescending(h => h.DataAlteracao)
                .FirstOrDefaultAsync();
        }

        public async Task AddHistoricoAsync(int pedidoId, StatusPedido statusAnterior, StatusPedido statusNovo)
        {
            var historico = new PedidoHistorico
            {
                PedidoId = pedidoId,
                StatusAnterior = statusAnterior,
                StatusNovo = statusNovo,
                DataAlteracao = DateTime.UtcNow
            };

            await _dbSet.AddAsync(historico);
            await _context.SaveChangesAsync();
        }
    }
}