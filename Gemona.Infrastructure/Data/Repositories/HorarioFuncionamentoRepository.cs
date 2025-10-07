using Microsoft.EntityFrameworkCore;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Infrastructure.Data.Context;
using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Infrastructure.Data.Repositories
{
    public class HorarioFuncionamentoRepository : BaseRepository<HorarioFuncionamento>, IHorarioFuncionamentoRepository
    {
        public HorarioFuncionamentoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<HorarioFuncionamento>> GetHorariosByEstabelecimentoAsync(int estabelecimentoId)
        {
            return await _dbSet
                .Where(h => h.EstabelecimentoId == estabelecimentoId && h.Ativo)
                .OrderBy(h => h.DiaSemana)
                .ToListAsync();
        }

        public async Task<HorarioFuncionamento?> GetHorarioByEstabelecimentoEDiaAsync(int estabelecimentoId, DiaSemana diaSemana)
        {
            return await _dbSet
                .FirstOrDefaultAsync(h => h.EstabelecimentoId == estabelecimentoId && 
                                         h.DiaSemana == diaSemana && h.Ativo);
        }

        public async Task<IEnumerable<HorarioFuncionamento>> GetEstabelecimentosAbertosAsync(DiaSemana diaSemana, TimeOnly horario)
        {
            return await _dbSet
                .Include(h => h.Estabelecimento)
                .Where(h => h.DiaSemana == diaSemana && 
                           !h.Fechado &&
                           h.HoraAbertura <= horario &&
                           h.HoraFechamento >= horario && 
                           h.Ativo)
                .ToListAsync();
        }

        public async Task<bool> EstabelecimentoAbertoAsync(int estabelecimentoId, DiaSemana diaSemana, TimeOnly horario)
        {
            return await _dbSet
                .AnyAsync(h => h.EstabelecimentoId == estabelecimentoId &&
                              h.DiaSemana == diaSemana &&
                              !h.Fechado &&
                              h.HoraAbertura <= horario &&
                              h.HoraFechamento >= horario && 
                              h.Ativo);
        }
    }
}