using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.DTOs.Request.Profissional
{
    public class UpdateProfissionalRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public Base64ImageDto? ImagemPerfil { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}