using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Request.Avaliacao
{
    public class UpdateAvaliacaoRequest
    {
        public NotaAvaliacao Nota { get; set; }
        public string? Comentario { get; set; }
        public string? ImagemComentarioUrl { get; set; }
    }
}