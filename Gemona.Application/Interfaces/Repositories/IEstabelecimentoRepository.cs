using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Application.Interfaces.Repositories
{
    public interface IEstabelecimentoRepository : IBaseRepository<Estabelecimento>
    {
        Task<Estabelecimento?> GetByCnpjAsync(Cnpj cnpj);
        Task<bool> CnpjExistsAsync(Cnpj cnpj);
        Task<Estabelecimento?> GetEstabelecimentoCompletoAsync(int estabelecimentoId);
        Task<IEnumerable<Estabelecimento>> GetEstabelecimentosByProfissionalAsync(int profissionalId);
        Task<IEnumerable<Estabelecimento>> GetEstabelecimentosByCidadeAsync(string cidade);
        Task<IEnumerable<Estabelecimento>> GetEstabelecimentosProximosAsync(decimal latitude, decimal longitude, double raioKm);
    }
}