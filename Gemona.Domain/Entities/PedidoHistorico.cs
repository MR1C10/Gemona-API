namespace Gemona.Domain.Entities
{
    public class PedidoHistorico : BaseEntity
    {
        public int PedidoHistoricoId { get; set; }
        public int PedidoId { get; set; }
        public string StatusAnterior { get; set; } = string.Empty;
        public string StatusNovo { get; set; } = string.Empty;
        public DateTime DataAlteracao { get; set; }

        public virtual Pedido Pedido { get; set; } = null!;
    }
}