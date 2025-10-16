using Gemona.Application.DTOs.Shared;
using Gemona.Application.DTOs.Response.Cliente;
using Gemona.Application.DTOs.Response.Servico;
using Gemona.Application.DTOs.Response.Avaliacao;
using Gemona.Domain.Enums;

namespace Gemona.Application.DTOs.Response.Pedido
{
    public class PedidoCompletoResponse : BaseResponse
    {
        public int PedidoId { get; set; }
        public ClienteResponse? Cliente { get; set; }
        public ServicoCompletoResponse? Servico { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public DateTime? DataAgendamento { get; set; }
        public decimal? ValorFinal { get; set; }
        public StatusPedido Status { get; set; }
        public string StatusTexto => Status.ToString();
        
        public List<HistoricoResponse> Historico { get; set; } = new();
        
        public AvaliacaoResponse? Avaliacao { get; set; }
    }
    
    public class HistoricoResponse
    {
        public StatusPedido StatusAnterior { get; set; }
        public StatusPedido StatusNovo { get; set; }
        public DateTime DataAlteracao { get; set; }
    }
}