using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class CategoriaRepository : BaseRepository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Categoria?> GetCategoriaWithSubCategoriasAsync(int categoriaId)
        {
            return await _dbSet
                .Include(c => c.SubCategorias)
                .FirstOrDefaultAsync(c => c.CategoriaId == categoriaId);
        }

        public async Task<IEnumerable<Categoria>> GetCategoriasWithSubCategoriasAsync()
        {
            return await _dbSet
                .Include(c => c.SubCategorias)
                .ToListAsync();
        }

        public async Task<Categoria?> GetByNomeAsync(string nome)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Nome == nome);
        }

        public async Task<bool> NomeExistsAsync(string nome)
        {
            return await _dbSet
                .AnyAsync(c => c.Nome == nome);
        }
    }
}