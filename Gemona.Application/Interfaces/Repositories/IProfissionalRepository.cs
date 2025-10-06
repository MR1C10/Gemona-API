using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Application.Interfaces.Repositories
{
    public interface IProfissionalRepository : IBaseRepository<Profissional>
    {
        Task<Profissional?> GetByEmailAsync(string email);
        Task<Profissional?> GetByCpfAsync(Cpf cpf);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> CpfExistsAsync(Cpf cpf);
        Task<Profissional?> GetProfissionalWithEstabelecimentoAsync(int profissionalId);
    }
}