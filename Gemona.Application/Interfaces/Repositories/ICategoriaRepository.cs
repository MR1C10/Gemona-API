using Gemona.Domain.Entities;

namespace Gemona.Application.Interfaces.Repositories
{
    public interface ICategoriaRepository : IBaseRepository<Categoria>
    {
        Task<Categoria?> GetCategoriaWithSubCategoriasAsync(int categoriaId);
        Task<IEnumerable<Categoria>> GetCategoriasWithSubCategoriasAsync();
        Task<Categoria?> GetByNomeAsync(string nome);
        Task<bool> NomeExistsAsync(string nome);
    }
}