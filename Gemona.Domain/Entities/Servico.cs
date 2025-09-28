namespace Gemona.Domain.Entities
{
    public class Servico : BaseEntity
    {
        public int ServicoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public int SubCategoriaId { get; set; }
        public decimal Preco { get; set; }
        public string? ImagemServicoUrl { get; set; }

        public virtual SubCategoria SubCategoria { get; set; } = null!;
        public virtual Estabelecimento Estabelecimento { get; set; } = null!;
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}