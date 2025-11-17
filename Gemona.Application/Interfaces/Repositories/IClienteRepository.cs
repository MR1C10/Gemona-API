using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Application.Interfaces.Repositories
{
    public interface IClienteRepository : IBaseRepository<Cliente>
    {
        Task<Cliente?> GetByEmailAsync(string email);
        Task<Cliente?> GetByCpfAsync(Cpf cpf);
        Task<Cliente?> GetByCpfAsync(string cpf);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> CpfExistsAsync(Cpf cpf);
        Task<bool> CpfExistsAsync(string cpf);
        Task<Cliente?> GetClienteWithEnderecoAsync(int clienteId);
        Task<IEnumerable<Cliente>> GetClientesByIdadeAsync(int idadeMinima, int idadeMaxima);
    }
}