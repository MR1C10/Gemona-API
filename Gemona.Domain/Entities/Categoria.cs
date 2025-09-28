namespace Gemona.Domain.Entities
{
    public class Categoria : BaseEntity
    {
        public int CategoriaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? ImagemCategorialUrl { get; set; }

        public virtual ICollection<SubCategoria> SubCategorias { get; set; } = new List<SubCategoria>();
    }
}