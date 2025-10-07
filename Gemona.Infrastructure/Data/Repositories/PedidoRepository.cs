using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class PedidoRepository : BaseRepository<Pedido>, IPedidoRepository
    {
        public PedidoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Pedido>> GetPedidosByClienteAsync(int clienteId)
        {
            return await _dbSet
                .Include(p => p.Servico)
                .Where(p => p.ClienteId == clienteId && p.Ativo)
                .OrderByDescending(p => p.DataSolicitacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pedido>> GetPedidosByEstabelecimentoAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Include(p => p.Cliente)
                .Include(p => p.Servico)
                .Where(p => p.Servico.EstabelecimentoId == estabelecimentoId && p.Ativo)
                .OrderByDescending(p => p.DataSolicitacao)
                .ToListAsync();
        }

        public async Task<IEnumerable<Pedido>> GetPedidosByStatusAsync(StatusPedido status)
        {
            return await _dbSet
                .Include(p => p.Cliente)
                .Include(p => p.Servico)
                .Where(p => p.Status == status && p.Ativo)
                .ToListAsync();
        }

        public async Task<Pedido?> GetPedidoCompletoAsync(int pedidoId)
        {
            return await _dbSet
                .Include(p => p.Cliente)
                .Include(p => p.Servico)
                .ThenInclude(s => s.Estabelecimento)
                .Include(p => p.Historicos)
                .Include(p => p.Avaliacao)
                .FirstOrDefaultAsync(p => p.PedidoId == pedidoId && p.Ativo);
        }

        public async Task<IEnumerable<Pedido>> GetPedidosPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _dbSet
                .Where(p => p.DataSolicitacao >= dataInicio && 
                           p.DataSolicitacao <= dataFim && p.Ativo)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalVendasEstabelecimentoAsync(int estabelecimentoId, DateTime dataInicio, DateTime dataFim)
        {
            return await _dbSet
                .Include(p => p.Servico)
                .Where(p => p.Servico.EstabelecimentoId == estabelecimentoId &&
                            p.Status == StatusPedido.Concluido &&
                            p.DataConclusao >= dataInicio &&
                            p.DataConclusao <= dataFim &&
                            p.Ativo)
                .SumAsync(p => p.ValorFinal ?? 0);
        }
    }
}