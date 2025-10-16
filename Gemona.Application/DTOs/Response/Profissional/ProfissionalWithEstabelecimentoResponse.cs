using Gemona.Application.DTOs.Shared;
using Gemona.Application.DTOs.Response.Estabelecimento;

namespace Gemona.Application.DTOs.Response.Profissional
{
    public class ProfissionalWithEstabelecimentoResponse : BaseResponse
    {
        public int ProfissionalId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string? ImagemPerfilUrl { get; set; }
        public DateTime DataNascimento { get; set; }
        public EstabelecimentoResponse? Estabelecimento { get; set; }
    }
}