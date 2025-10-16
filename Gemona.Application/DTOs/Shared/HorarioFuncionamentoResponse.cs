using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Shared
{
    public class HorarioFuncionamentoResponse
    {
        public DiaSemana DiaSemana { get; set; }
        public string DiaSemanaTexto => DiaSemana.ToString();
        public TimeOnly? HoraAbertura { get; set; }
        public TimeOnly? HoraFechamento { get; set; }
        public bool Fechado { get; set; }
        public string HorarioTexto => Fechado ? "Fechado" : $"{HoraAbertura} - {HoraFechamento}";
    }
}