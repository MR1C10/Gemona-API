using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Helpers;
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
                .Where(e => e.Cidade == cidade)
                .ToListAsync();
        }

        public async Task<IEnumerable<Endereco>> GetEnderecosByEstadoAsync(string estado)
        {
            return await _dbSet
                .Where(e => e.Estado == estado)
                .ToListAsync();
        }

        public async Task<Endereco?> GetEnderecoByCepAsync(Cep cep)
        {
            return await GetEnderecoByCepAsync(cep.Valor);
        }

        public async Task<Endereco?> GetEnderecoByCepAsync(string cep)
        {
            var cepObj = new Cep(cep);
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Cep == cepObj);
        }

        public async Task<IEnumerable<Endereco>> GetEnderecosProximosAsync(decimal latitude, decimal longitude, double raioKm)
        {
            var enderecos = await _dbSet.ToListAsync();

            // Filtra por distância usando Haversine
            return enderecos
                .Where(e => GeoHelper.CalcularDistancia(
                    latitude, longitude,
                    e.Latitude, e.Longitude
                ) <= raioKm)
                .OrderBy(e => GeoHelper.CalcularDistancia(
                    latitude, longitude,
                    e.Latitude, e.Longitude
                ))
                .ToList();
        }
    }
}