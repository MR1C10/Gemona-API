using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class // ← MUDOU: BaseEntity para class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllActiveAsync()
        {
            // Para entidades com Identity, verificar se tem propriedade Ativo
            var activeProperty = typeof(T).GetProperty("Ativo");
            if (activeProperty != null)
            {
                return await _dbSet.Where(x => EF.Property<bool>(x, "Ativo")).ToListAsync();
            }
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return await Task.FromResult(entity);
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                // Verificar se tem propriedade Ativo
                var ativoProperty = typeof(T).GetProperty("Ativo");
                if (ativoProperty != null)
                {
                    ativoProperty.SetValue(entity, false); // soft delete
                }
            }
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            // Verificar se tem propriedade Ativo
            var activeProperty = typeof(T).GetProperty("Ativo");
            if (activeProperty != null)
            {
                return await _dbSet.AnyAsync(x => EF.Property<bool>(x, "Ativo") && EF.Property<int>(x, "Id") == id);
            }
            return await _dbSet.AnyAsync(x => EF.Property<int>(x, "Id") == id);
        }

        public virtual async Task<int> CountAsync()
        {
            var activeProperty = typeof(T).GetProperty("Ativo");
            if (activeProperty != null)
            {
                return await _dbSet.CountAsync(x => EF.Property<bool>(x, "Ativo"));
            }
            return await _dbSet.CountAsync();
        }

        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}