using Gemona.Domain.Enums;

namespace Gemona.Domain.Entities
{
    public class HorarioFuncionamento : BaseEntity
    {
        public int HorarioId { get; set; }
        public int EstabelecimentoId { get; set; }
        public DiaSemana DiaSemana { get; set; }
        public TimeOnly? HoraAbertura { get; set; }
        public TimeOnly? HoraFechamento { get; set; }
        public bool Fechado { get; set; } = false;

        public virtual Estabelecimento Estabelecimento { get; set; } = null!;
    }
}