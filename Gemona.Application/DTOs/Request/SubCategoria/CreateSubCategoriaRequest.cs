namespace Gemona.Application.DTOs.Request.SubCategoria
{
    public class CreateSubCategoriaRequest
    {
        public string Nome { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string? ImagemSubcategoriaUrl { get; set; }
    }
}