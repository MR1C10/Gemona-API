using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Application.Interfaces.Repositories
{
    public interface IHorarioFuncionamentoRepository : IBaseRepository<HorarioFuncionamento>
    {
        Task<IEnumerable<HorarioFuncionamento>> GetHorariosByEstabelecimentoAsync(int estabelecimentoId);
        Task<HorarioFuncionamento?> GetHorarioByEstabelecimentoEDiaAsync(int estabelecimentoId, DiaSemana diaSemana);
        Task<IEnumerable<HorarioFuncionamento>> GetEstabelecimentosAbertosAsync(DiaSemana diaSemana, TimeOnly horario);
        Task<bool> EstabelecimentoAbertoAsync(int estabelecimentoId, DiaSemana diaSemana, TimeOnly horario);
    }
}