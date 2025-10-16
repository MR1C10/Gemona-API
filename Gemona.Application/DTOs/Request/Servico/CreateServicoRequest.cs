namespace Gemona.Application.DTOs.Request.Servico
{
    public class CreateServicoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int SubCategoriaId { get; set; }
        public decimal Preco { get; set; }
        public string? ImagemServicoUrl { get; set; }
        public int EstabelecimentoId { get; set; }
    }
}