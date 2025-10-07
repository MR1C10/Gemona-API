using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class ServicoRepository : BaseRepository<Servico>, IServicoRepository
    {
        public ServicoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Servico>> GetServicosByEstabelecimentoAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Where(s => s.EstabelecimentoId == estabelecimentoId && s.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Servico>> GetServicosByCategoriaAsync(int categoriaId)
        {
            return await _dbSet
                .Include(s => s.SubCategoria)
                .Where(s => s.SubCategoria.CategoriaId == categoriaId && s.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Servico>> GetServicosBySubCategoriaAsync(int subCategoriaId)
        {
            return await _dbSet
                .Where(s => s.SubCategoriaId == subCategoriaId && s.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Servico>> GetServicosByFaixaPrecoAsync(decimal precoMinimo, decimal precoMaximo)
        {
            return await _dbSet
                .Where(s => s.Preco >= precoMinimo && s.Preco <= precoMaximo && s.Ativo)
                .ToListAsync();
        }

        public async Task<Servico?> GetServicoCompletoAsync(int servicoId)
        {
            return await _dbSet
                .Include(s => s.SubCategoria)
                .ThenInclude(sc => sc.Categoria)
                .Include(s => s.Estabelecimento)
                .ThenInclude(e => e.Endereco)
                .FirstOrDefaultAsync(s => s.ServicoId == servicoId && s.Ativo);
        }

        public async Task<IEnumerable<Servico>> BuscarServicosAsync(string termo)
        {
            return await _dbSet
                .Include(s => s.SubCategoria)
                .ThenInclude(sc => sc.Categoria)
                .Where(s => (s.Nome.Contains(termo) || 
                            s.Descricao!.Contains(termo) ||
                            s.SubCategoria.Nome.Contains(termo) ||
                            s.SubCategoria.Categoria.Nome.Contains(termo)) && s.Ativo) 
                .ToListAsync();
        }
    }
}