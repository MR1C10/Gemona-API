using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Request.Pedido
{
    public class UpdateStatusPedidoRequest
    {
        public StatusPedido NovoStatus { get; set; }
        public DateTime? DataAgendamento { get; set; }
        public DateTime? DataConclusao { get; set; }
        public decimal? ValorFinal { get; set; }
        public string? Observacoes { get; set; }
    }
}