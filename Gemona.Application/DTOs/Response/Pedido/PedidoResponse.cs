using Gemona.Application.DTOs.Shared;
using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Response.Pedido
{
    public class PedidoResponse : BaseResponse
    {
        public int PedidoId { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNome { get; set; } = string.Empty;
        public int ServicoId { get; set; }
        public string ServicoNome { get; set; } = string.Empty;
        public decimal ServicoPreco { get; set; }
        public string EstabelecimentoNome { get; set; } = string.Empty;
        public DateTime DataSolicitacao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public DateTime? DataAgendamento { get; set; }
        public decimal? ValorFinal { get; set; }
        public StatusPedido Status { get; set; }
        public string StatusTexto => Status.ToString();
    }
}