using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Application.Interfaces.Repositories
{
    public interface IEnderecoRepository : IBaseRepository<Endereco>
    {
        Task<IEnumerable<Endereco>> GetEnderecosByCidadeAsync(string cidade);
        Task<IEnumerable<Endereco>> GetEnderecosByEstadoAsync(string estado);
        Task<Endereco?> GetEnderecoByCepAsync(Cep cep);
        Task<Endereco?> GetEnderecoByCepAsync(string cep);
        Task<IEnumerable<Endereco>> GetEnderecosProximosAsync(decimal latitude, decimal longitude, double raioKm);
    }
}