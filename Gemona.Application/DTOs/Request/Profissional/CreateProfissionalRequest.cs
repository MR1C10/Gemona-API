using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.DTOs.Request.Profissional
{
    public class CreateProfissionalRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public Base64ImageDto? ImagemPerfil { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Senha { get; set; } = string.Empty;
    }
}