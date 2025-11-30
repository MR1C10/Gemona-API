using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.DTOs.Response.Endereco
{
    public class EnderecoResponse : BaseResponse
    {
        public int EnderecoId { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string? Numero { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string EnderecoCompleto => $"{Rua}, {Numero} - {Bairro}, {Cidade}/{Estado}";
        public string CepFormatado => $"{Cep.Substring(0, 5)}-{Cep.Substring(5, 3)}";
    }
}