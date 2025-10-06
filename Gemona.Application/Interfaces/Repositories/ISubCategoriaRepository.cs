using Gemona.Domain.Entities;

namespace Gemona.Application.Interfaces.Repositories
{
    public interface ISubCategoriaRepository : IBaseRepository<SubCategoria>
    {
        Task<IEnumerable<SubCategoria>> GetSubCategoriasByCategoriaAsync(int categoriaId);
        Task<SubCategoria?> GetSubCategoriaWithServicosAsync(int subCategoriaId);
        Task<SubCategoria?> GetByNomeAsync(string nome);
        Task<bool> NomeExistsAsync(string nome, int categoriaId);
    }
}