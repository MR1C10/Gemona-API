using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Request.Avaliacao
{
    public class FiltrarAvaliacoesRequest
    {
        public int? ClienteId { get; set; }
        public int? EstabelecimentoId { get; set; }
        public NotaAvaliacao? Nota { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}