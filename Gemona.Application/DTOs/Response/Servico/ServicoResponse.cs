using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.DTOs.Response.Servico
{
    public class ServicoResponse : BaseResponse
    {
        public int ServicoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int SubCategoriaId { get; set; }
        public string SubCategoriaNome { get; set; } = string.Empty;
        public string CategoriaNome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public string? ImagemServicoUrl { get; set; }
        public int EstabelecimentoId { get; set; }
        public string EstabelecimentoNome { get; set; } = string.Empty;
    }
}