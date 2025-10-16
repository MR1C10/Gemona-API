using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Request.Pedido
{
    public class FiltrarPedidosRequest
    {
        public int? ClienteId { get; set; }
        public int? EstabelecimentoId { get; set; }
        public StatusPedido? Status { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}