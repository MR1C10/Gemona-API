namespace Gemona.Application.DTOs.Request.Pedido
{
    public class CreatePedidoRequest
    {
        public int ClienteId { get; set; }
        public int ServicoId { get; set; }
        public DateTime? DataAgendamento { get; set; }
        public string? Observacoes { get; set; }
    }
}