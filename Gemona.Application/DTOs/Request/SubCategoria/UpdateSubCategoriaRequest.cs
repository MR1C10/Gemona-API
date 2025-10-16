namespace Gemona.Application.DTOs.Request.SubCategoria
{
    public class UpdateSubCategoriaRequest
    {
        public string Nome { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string? ImagemSubcategoriaUrl { get; set; }
    }
}