using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.DTOs.Response.Profissional
{
    public class ProfissionalResponse : BaseResponse
    {
        public int ProfissionalId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string? ImagemPerfilUrl { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}