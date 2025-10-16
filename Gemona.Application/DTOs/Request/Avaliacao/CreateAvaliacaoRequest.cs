using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Request.Avaliacao
{
    public class CreateAvaliacaoRequest
    {
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        public NotaAvaliacao Nota { get; set; }
        public string? Comentario { get; set; }
        public string? ImagemComentarioUrl { get; set; }
    }
}