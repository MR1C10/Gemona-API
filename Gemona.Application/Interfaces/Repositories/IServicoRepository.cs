using Gemona.Domain.Entities;

namespace Gemona.Application.Interfaces.Repositories
{
    public interface IServicoRepository : IBaseRepository<Servico>
    {
        Task<IEnumerable<Servico>> GetServicosByEstabelecimentoAsync(int estabelecimentoId);
        Task<IEnumerable<Servico>> GetServicosByCategoriaAsync(int categoriaId);
        Task<IEnumerable<Servico>> GetServicosBySubCategoriaAsync(int subCategoriaId);
        Task<IEnumerable<Servico>> GetServicosByFaixaPrecoAsync(decimal precoMinimo, decimal precoMaximo);
        Task<Servico?> GetServicoCompletoAsync(int servicoId);
        Task<IEnumerable<Servico>> BuscarServicosAsync(string termo);
    }
}