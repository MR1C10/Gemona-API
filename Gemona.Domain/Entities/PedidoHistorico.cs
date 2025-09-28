using Gemona.Domain.Enums;

namespace Gemona.Domain.Entities
{
    public class PedidoHistorico : BaseEntity
    {
        public int PedidoHistoricoId { get; set; }
        public int PedidoId { get; set; }
        public StatusPedido StatusAnterior { get; set; }
        public StatusPedido StatusNovo { get; set; }
        public DateTime DataAlteracao { get; set; }

        public virtual Pedido Pedido { get; set; } = null!;
    }
}