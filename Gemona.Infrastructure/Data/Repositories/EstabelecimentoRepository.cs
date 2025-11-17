using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Helpers;
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
            return await GetByCnpjAsync(cnpj.Valor);
        }

        public async Task<Estabelecimento?> GetByCnpjAsync(string cnpj)
        {
            var cnpjObj = new Cnpj(cnpj);
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Cnpj == cnpjObj);
        }

        public async Task<bool> CnpjExistsAsync(Cnpj cnpj)
        {
            return await CnpjExistsAsync(cnpj.Valor);
        }

        public async Task<bool> CnpjExistsAsync(string cnpj)
        {
            var cnpjObj = new Cnpj(cnpj);
            return await _dbSet
                .AnyAsync(e => e.Cnpj == cnpjObj);
        }

        public async Task<Estabelecimento?> GetEstabelecimentoCompletoAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Include(e => e.Endereco)
                .Include(e => e.Profissional)
                .Include(e => e.HorariosFuncionamento)
                .Include(e => e.Servicos)
                .FirstOrDefaultAsync(e => e.EstabelecimentoId == estabelecimentoId);
        }

        public async Task<IEnumerable<Estabelecimento>> GetEstabelecimentosByProfissionalAsync(int profissionalId)
        {
            return await _dbSet
                .Where(e => e.ProfissionalId == profissionalId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Estabelecimento>> GetEstabelecimentosByCidadeAsync(string cidade)
        {
            return await _dbSet
                .Include(e => e.Endereco)
                .Where(e => e.Endereco.Cidade == cidade)
                .ToListAsync();
        }

        public async Task<IEnumerable<Estabelecimento>> GetEstabelecimentosProximosAsync(decimal latitude, decimal longitude, double raioKm)
        {
            var estabelecimentos = await _dbSet
                .Include(e => e.Endereco)
                .ToListAsync();

            // Filtra por distância usando Haversine
            return estabelecimentos
                .Where(e => GeoHelper.CalcularDistancia(
                    latitude, longitude,
                    e.Endereco.Latitude, e.Endereco.Longitude
                ) <= raioKm)
                .OrderBy(e => GeoHelper.CalcularDistancia(
                    latitude, longitude,
                    e.Endereco.Latitude, e.Endereco.Longitude
                ))
                .ToList();
        }

        public async Task<IEnumerable<Estabelecimento>> BuscarEstabelecimentosAsync(string termo)
        {
            return await _dbSet
                .Include(e => e.Endereco)
                .Where(e => e.Nome.Contains(termo) || 
                           e.Descricao!.Contains(termo) ||
                           e.Endereco.Cidade.Contains(termo) ||
                           e.Endereco.Bairro.Contains(termo))
                .ToListAsync();
        }
    }
}