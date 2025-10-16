namespace Gemona.Application.DTOs.Request.Categoria
{
    public class UpdateCategoriaRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? ImagemCategoriaUrl { get; set; } 
    }
}