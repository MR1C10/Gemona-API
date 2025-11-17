using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.DTOs.Request.Servico
{
    public class UpdateServicoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int SubCategoriaId { get; set; }
        public decimal Preco { get; set; }
        public Base64ImageDto? ImagemServico { get; set; }
    }
}