using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.DTOs.Response.Categoria
{
    public class CategoriaResponse : BaseResponse
    {
        public int Categoria { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? ImagemCategoriaUrl { get; set; }
    }
}