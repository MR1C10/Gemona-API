using Gemona.Application.DTOs.Shared;
using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Request.Avaliacao
{
    public class UpdateAvaliacaoRequest
    {
        public NotaAvaliacao Nota { get; set; }
        public string? Comentario { get; set; }
        public Base64ImageDto? ImagemComentario { get; set; }
    }
}