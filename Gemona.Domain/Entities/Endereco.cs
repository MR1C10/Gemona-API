namespace Gemona.Domain.Entities
{
    public class Endereco : BaseEntity
    {
        public int EnderecoId { get; set; }
        public string Rua { get; set; } = string.Empty;
        public string? Numero { get; set; }
        public string Bairro { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public decimal Altitude { get; set; }
        public decimal Longitude { get; set; }

        public Cliente? Cliente { get; set; }
        public Estabelecimento? Estabelecimento { get; set; }
    }
}