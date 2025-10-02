using Gemona.Domain.Enums;

namespace Gemona.Domain.Entities
{
    public class Avaliacao : BaseEntity
    {
        public int AvaliacaoId { get; set; }
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        public NotaAvaliacao Nota { get; set; } 
        public string? Comentario { get; set; }
        public DateTime Data { get; set; }
        public string? ImagemAvaliacaoUrl { get; set; }

        public virtual Pedido Pedido { get; set; } = null!;
        public virtual Cliente Cliente { get; set; } = null!;
    }
} 