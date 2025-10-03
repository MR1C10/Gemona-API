namespace Gemona.Domain.Entities
{
    public class SubCategoria : BaseEntity
    {
        public int SubCategoriaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string? ImagemSubcategoriaUrl { get; set; }

        public virtual Categoria Categoria { get; set; } = null!;
        public virtual ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    }
}