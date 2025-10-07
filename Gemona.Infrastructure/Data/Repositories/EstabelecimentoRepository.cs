using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class EstabelecimentoRepository : BaseRepository<Estabelecimento>, IEstabelecimentoRepository
    {
        public EstabelecimentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Estabelecimento?> GetByCnpjAsync(Cnpj cnpj)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Cnpj.Valor == cnpj.Valor && e.Ativo);
        }

        public async Task<bool> CnpjExistsAsync(Cnpj cnpj)
        {
            return await _dbSet
                .AnyAsync(e => e.Cnpj.Valor == cnpj.Valor && e.Ativo);
        }

        public async Task<Estabelecimento?> GetEstabelecimentoCompletoAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Include(e => e.Endereco)
                .Include(e => e.Profissional)
                .Include(e => e.HorariosFuncionamento)
                .Include(e => e.Servicos)
                .FirstOrDefaultAsync(e => e.EstabelecimentoId == estabelecimentoId && e.Ativo);
        }

        public async Task<IEnumerable<Estabelecimento>> GetEstabelecimentosByProfissionalAsync(int profissionalId)
        {
            return await _dbSet
                .Where(e => e.ProfissionalId == profissionalId && e.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Estabelecimento>> GetEstabelecimentosByCidadeAsync(string cidade)
        {
            return await _dbSet
                .Include(e => e.Endereco)
                .Where(e => e.Endereco.Cidade == cidade && e.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Estabelecimento>> GetEstabelecimentosProximosAsync(decimal latitude, decimal longitude, double raioKm)
        {
            // depois implemento o filtro por distacia
            // por enquanto retorna todos
            return await _dbSet
                .Include(e => e.Endereco)
                .Where(e => e.Ativo)
                .ToListAsync();
        }
    }
}