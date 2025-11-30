using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Helpers;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;
using System.Linq;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class EstabelecimentoRepository : BaseRepository<Estabelecimento>, IEstabelecimentoRepository
    {
        public EstabelecimentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        private IQueryable<Estabelecimento> GetFullQuery()
        {
            return _dbSet
                .AsNoTracking() // Melhora a performance para consultas de leitura
                .Include(e => e.Endereco)
                .Include(e => e.Profissional)
                .Include(e => e.HorariosFuncionamento);
        }

        public async Task<Estabelecimento?> GetByCnpjAsync(Cnpj cnpj)
        {
            return await GetByCnpjAsync(cnpj.Valor);
        }

        public async Task<Estabelecimento?> GetByCnpjAsync(string cnpj)
        {
            var cnpjObj = new Cnpj(cnpj);
            return await GetFullQuery()
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
            return await GetFullQuery()
                .Include(e => e.Servicos) // Inclui Serviços apenas aqui
                .FirstOrDefaultAsync(e => e.EstabelecimentoId == estabelecimentoId);
        }

        public async Task<IEnumerable<Estabelecimento>> GetEstabelecimentosByProfissionalAsync(int profissionalId)
        {
            return await GetFullQuery()
                .Where(e => e.ProfissionalId == profissionalId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Estabelecimento>> GetEstabelecimentosByCidadeAsync(string cidade)
        {
            return await GetFullQuery()
                .Where(e => e.Endereco != null && e.Endereco.Cidade == cidade) // Adicionado null check para e.Endereco
                .ToListAsync();
        }

        public async Task<IEnumerable<Estabelecimento>> GetEstabelecimentosProximosAsync(decimal latitude, decimal longitude, double raioKm)
        {
            var estabelecimentos = await GetFullQuery().ToListAsync();

            // Filtra por distância usando Haversine
            return estabelecimentos
                .Where(e => e.Endereco != null && GeoHelper.CalcularDistancia( // Adicionado null check para e.Endereco
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
            return await GetFullQuery()
                .Where(e => e.Nome.Contains(termo) || 
                           (e.Descricao != null && e.Descricao.Contains(termo)) || // Adicionado null check
                           (e.Endereco != null && (e.Endereco.Cidade.Contains(termo) || e.Endereco.Bairro.Contains(termo)))) // Adicionado null check
                .ToListAsync();
        }

        public override async Task<IEnumerable<Estabelecimento>> GetAllActiveAsync()
        {
            return await GetFullQuery()
                .Where(e => e.Ativo)
                .ToListAsync();
        }
    }
}