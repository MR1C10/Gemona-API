using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class EnderecoRepository : BaseRepository<Endereco>, IEnderecoRepository
    {
        public EnderecoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Endereco>> GetEnderecosByCidadeAsync(string cidade)
        {
            return await _dbSet
                .Where(e => e.Cidade == cidade && e.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Endereco>> GetEnderecosByEstadoAsync(string estado)
        {
            return await _dbSet
                .Where(e => e.Estado == estado && e.Ativo)
                .ToListAsync();
        }

        public async Task<Endereco?> GetEnderecoByCepAsync(Cep cep)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Cep.Valor == cep.Valor && e.Ativo);
        }

        public async Task<IEnumerable<Endereco>> GetEnderecosProximosAsync(decimal latitude, decimal longitude, double raioKm)
        {
            // Implementação simples por enquanto - implementar cálculo de distância depois
            return await _dbSet
                .Where(e => e.Ativo)
                .ToListAsync();
        }
    }
}