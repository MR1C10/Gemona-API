namespace Gemona.Application.DTOs.Request.HorarioFuncionamento
{
    public class UpdateHorariosEstabelecimentoRequest
    {
        public int EstabelecimentoId { get; set; }
        public List<UpdateHorarioRequest> Horarios { get; set; } = new();
    }
}