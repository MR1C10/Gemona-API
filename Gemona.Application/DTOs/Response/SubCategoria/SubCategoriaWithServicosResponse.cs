using Gemona.Application.DTOs.Shared;
using Gemona.Application.DTOs.Response.Servico;

namespace Gemona.Application.DTOs.Response.SubCategoria
{
    public class SubCategoriaWithServicosResponse : BaseResponse
    {
        public int SubCategoriaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
        public string? ImagemSubcategoriaUrl { get; set; }
        public IEnumerable<ServicoResponse> Servicos { get; set; } = new List<ServicoResponse>();
    }
}