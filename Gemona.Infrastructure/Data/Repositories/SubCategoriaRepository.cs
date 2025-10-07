using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class SubCategoriaRepository : BaseRepository<SubCategoria>, ISubCategoriaRepository
    {
        public SubCategoriaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SubCategoria>> GetSubCategoriasByCategoriaAsync(int categoriaId)
        {
            return await _dbSet
                .Where(sc => sc.CategoriaId == categoriaId && sc.Ativo)
                .ToListAsync();
        }

        public async Task<SubCategoria?> GetSubCategoriaWithServicosAsync(int subCategoriaId)
        {
            return await _dbSet
                .Include(sc => sc.Servicos.Where(s => s.Ativo))
                .FirstOrDefaultAsync(sc => sc.SubCategoriaId == subCategoriaId && sc.Ativo);
        }

        public async Task<SubCategoria?> GetByNomeAsync(string nome)
        {
            return await _dbSet
                .FirstOrDefaultAsync(sc => sc.Nome == nome && sc.Ativo);
        }

        public async Task<bool> NomeExistsAsync(string nome, int categoriaId)
        {
            return await _dbSet
                .AnyAsync(sc => sc.Nome == nome && sc.CategoriaId == categoriaId && sc.Ativo);
        }
    }
}