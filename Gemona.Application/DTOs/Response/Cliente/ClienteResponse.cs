using Gemona.Application.DTOs.Shared;
using Gemona.Application.DTOs.Response.Endereco;

namespace Gemona.Application.DTOs.Response.Cliente
{
    public class ClienteResponse : BaseResponse
    {
        public int ClienteId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string? ImagemPerfilUrl { get; set; }
        public DateTime DataNascimento { get; set; }
        public EnderecoResponse? Endereco { get; set; }
    }
}