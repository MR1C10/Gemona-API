using Gemona.Application.DTOs.Shared;
using Gemona.Application.DTOs.Response.Endereco;

namespace Gemona.Application.DTOs.Response.Estabelecimento
{
    public class EstabelecimentoResponse : BaseResponse
    {
        public int EstabelecimentoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string Cnpj { get; set; } = string.Empty;
        public string? ImagemEstabelecimentoUrl { get; set; }
        public int ProfissionalId { get; set; }
        public string ProfissionalNome { get; set; } = string.Empty;
        public EnderecoResponse? Endereco { get; set; }
        public List<HorarioFuncionamentoResponse> Horarios { get; set; } = new();
    }
}