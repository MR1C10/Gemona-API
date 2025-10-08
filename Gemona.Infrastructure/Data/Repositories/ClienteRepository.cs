using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class ClienteRepository : BaseRepository<Cliente>, IClienteRepository
    {
        public ClienteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Cliente?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<Cliente?> GetByCpfAsync(Cpf cpf)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Cpf.Valor == cpf.Valor);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet
                .AnyAsync(c => c.Email == email);
        }

        public async Task<bool> CpfExistsAsync(Cpf cpf)
        {
            return await _dbSet
                .AnyAsync(c => c.Cpf.Valor == cpf.Valor);
        }

        public async Task<Cliente?> GetClienteWithEnderecoAsync(int clienteId)
        {
            return await _dbSet
                .Include(c => c.Endereco)
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId);
        }

        public async Task<IEnumerable<Cliente>> GetClientesByIdadeAsync(int idadeMinima, int idadeMaxima)
        {
            var dataLimiteMax = DateTime.Today.AddYears(-idadeMinima);
            var dataLimiteMin = DateTime.Today.AddYears(-idadeMaxima - 1);

            return await _dbSet
                .Where(c => c.DataNascimento >= dataLimiteMin && 
                        c.DataNascimento <= dataLimiteMax)
                .ToListAsync();
        }
    }
}