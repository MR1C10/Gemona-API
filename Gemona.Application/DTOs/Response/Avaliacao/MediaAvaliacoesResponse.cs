namespace Gemona.Application.DTOs.Response.Avaliacao
{
    public class MediaAvaliacoesResponse
    {
        public int EstabelecimentoId { get; set; }
        public string EstabelecimentoNome { get; set; } = string.Empty;
        public double MediaGeral { get; set; }
        public int TotalAvaliacoes { get; set; }
        public Dictionary<int, int> DistribuicaoNotas { get; set; } = new(); // Nota -> Quantidade
        public List<AvaliacaoResponse> AvaliacoesRecentes { get; set; } = new();
    }
}