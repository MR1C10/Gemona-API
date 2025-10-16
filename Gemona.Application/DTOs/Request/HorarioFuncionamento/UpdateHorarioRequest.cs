using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Request.HorarioFuncionamento
{
    public class UpdateHorarioRequest
    {
        public DiaSemana DiaSemana { get; set; }
        public TimeOnly? HoraAbertura { get; set; }
        public TimeOnly? HoraFechamento { get; set; }
        public bool Fechado { get; set; } 
    }
}