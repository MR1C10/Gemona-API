namespace Gemona.Application.DTOs.Request.Estabelecimento
{
    public class UpdateEstabelecimentoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? ImagemEstabelecimentoUrl { get; set; }

        public string Rua { get; set; } = string.Empty;
        public string? Numero { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public List<HorarioFuncionamentoRequest> Horarios { get; set; } = new();
    }
}