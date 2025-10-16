using Gemona.Application.DTOs.Shared;
using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Response.Avaliacao
{
    public class AvaliacaoResponse : BaseResponse
    {
        public int AvaliacaoId { get; set; }
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNome { get; set; } = string.Empty;
        public string? ClienteImagemUrl { get; set; }
        public NotaAvaliacao Nota { get; set; }
        public int NotaNumero => (int)Nota;
        public string? Comentario { get; set; }
        public DateTime Data { get; set; }
        public string? ImagemComentarioUrl { get; set; }
        public string ServicoNome { get; set; } = string.Empty;
        public string EstabelecimentoNome { get; set; } = string.Empty;
    }
}