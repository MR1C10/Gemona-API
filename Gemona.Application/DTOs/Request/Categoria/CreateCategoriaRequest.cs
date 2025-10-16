namespace Gemona.Application.DTOs.Request.Categoria
{
    public class CreateCategoriaRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? ImagemCategoriaUrl { get; set; }
    }
}