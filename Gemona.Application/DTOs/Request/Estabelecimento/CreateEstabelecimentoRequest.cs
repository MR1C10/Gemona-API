using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.DTOs.Request.Estabelecimento
{
    public class CreateEstabelecimentoRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string Cnpj { get; set; } = string.Empty;
        public string? ImagemEstabelecimentoUrl { get; set; }
        public int ProfissionalId { get; set; }

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

    public class HorarioFuncionamentoRequest
    {
        public int DiaSemana { get; set; } // 1-7
        public TimeOnly? HoraAbertura { get; set; }
        public TimeOnly? HoraFechamento { get; set; }
        public bool Fechado { get; set; }
    }
}