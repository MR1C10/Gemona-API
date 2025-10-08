using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class ProfissionalRepository : BaseRepository<Profissional>, IProfissionalRepository
    {
        public ProfissionalRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Profissional?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<Profissional?> GetByCpfAsync(Cpf cpf)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Cpf.Valor == cpf.Valor);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet
                .AnyAsync(p => p.Email == email);
        }

        public async Task<bool> CpfExistsAsync(Cpf cpf)
        {
            return await _dbSet
                .AnyAsync(p => p.Cpf.Valor == cpf.Valor);
        }

        public async Task<Profissional?> GetProfissionalWithEstabelecimentoAsync(int profissionalId)
        {
            return await _dbSet
                .Include(p => p.Estabelecimento)
                .FirstOrDefaultAsync(p => p.ProfissionalId == profissionalId);
        }
    }
}