namespace Gemona.Domain.Entities
{
    public class Pedido : BaseEntity
    {
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        public int ServicoId { get; set; }
        public DateTime? DataSolicitacao { get; set; }
        public DateTime? DataConclucao { get; set; }
        public decimal? ValorFinal { get; set; }
        public string Status { get; set; } = "Solicitado";

        public virtual Cliente Cliente { get; set; } = null!;
        public virtual Servico Servico { get; set; } = null!;
        public virtual ICollection<PedidoHistorico> Historicos { get; set; } = new List<PedidoHistorico>();
        public virtual Avaliacao? Avaliacao { get; set; }
    }
}