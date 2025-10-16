using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Request.HorarioFuncionamento
{
    public class CreateHorarioRequest
    {
        public int EstabelecimentoId { get; set; }
        public DiaSemana DiaSemana { get; set; }
        public TimeOnly? HoraAbertura { get; set; }
        public TimeOnly? HoraFechamento { get; set; }
        public bool Fechado { get; set; }
    }
}