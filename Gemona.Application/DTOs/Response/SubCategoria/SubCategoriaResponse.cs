using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.DTOs.Response.SubCategoria
{
    public class SubCategoriaResponse : BaseResponse
    {
        public int SubCategoriaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string CategoriaNome { get; set; } = string.Empty;
        public string? ImagemSubcategoriaUrl { get; set; }
    }
}