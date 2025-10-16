using Gemona.Application.DTOs.Shared;
using Gemona.Application.DTOs.Response.SubCategoria;

namespace Gemona.Application.DTOs.Response.Categoria
{
    public class CategoriaWithSubCategoriasResponse : BaseResponse
    {
        public int CategoriaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? ImagemCategoriaUrl { get; set; }
        public IEnumerable<SubCategoriaResponse> SubCategorias { get; set; } = new List<SubCategoriaResponse>();
    }
}