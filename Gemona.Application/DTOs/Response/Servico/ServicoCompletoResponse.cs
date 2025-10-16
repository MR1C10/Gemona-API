using Gemona.Application.DTOs.Shared;
using Gemona.Application.DTOs.Response.Estabelecimento;
using Gemona.Application.DTOs.Response.SubCategoria;

namespace Gemona.Application.DTOs.Response.Servico
{
    public class ServicoCompletoResponse : BaseResponse
    {
        public int ServicoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public decimal Preco { get; set; }
        public string? ImagemServicoUrl { get; set; }
        public SubCategoriaResponse? SubCategoria { get; set; }
        public EstabelecimentoResponse? Estabelecimento { get; set; }
    }
}