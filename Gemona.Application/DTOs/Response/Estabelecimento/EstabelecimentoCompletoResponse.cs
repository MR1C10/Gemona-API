using Gemona.Application.DTOs.Shared;
using Gemona.Application.DTOs.Response.Endereco;
using Gemona.Application.DTOs.Response.Profissional;
using Gemona.Application.DTOs.Response.Servico;

namespace Gemona.Application.DTOs.Response.Estabelecimento
{
    public class EstabelecimentoCompletoResponse : BaseResponse
    {
        public int EstabelecimentoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string Cnpj { get; set; } = string.Empty;
        public string? ImagemEstabelecimentoUrl { get; set; }
        public ProfissionalResponse? Profissional { get; set; }
        public EnderecoResponse? Endereco { get; set; }
        public List<HorarioFuncionamentoResponse> Horarios { get; set; } = new();
        public List<ServicoResponse> Servicos { get; set; } = new();
    }
}