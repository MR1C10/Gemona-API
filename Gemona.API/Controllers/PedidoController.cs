using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.DTOs.Request.Pedido;
using Gemona.Domain.Enums;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _pedidoService.GetAllAsync();
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _pedidoService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("cliente/{clienteId}")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<IActionResult> GetByCliente(int clienteId)
        {
            var result = await _pedidoService.GetPedidosByClienteAsync(clienteId);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("estabelecimento/{estabelecimentoId}")]
        [Authorize(Roles = "Profissional,Admin")]
        public async Task<IActionResult> GetByEstabelecimento(int estabelecimentoId)
        {
            var result = await _pedidoService.GetPedidosByEstabelecimentoAsync(estabelecimentoId);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(StatusPedido status)
        {
            var result = await _pedidoService.GetPedidosByStatusAsync(status);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("periodo")]
        public async Task<IActionResult> GetByPeriodo([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            var result = await _pedidoService.GetPedidosPorPeriodoAsync(dataInicio, dataFim);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("estabelecimento/{estabelecimentoId}/vendas")]
        [Authorize(Roles = "Profissional,Admin")]
        public async Task<IActionResult> GetTotalVendas(int estabelecimentoId, [FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            var result = await _pedidoService.GetTotalVendasEstabelecimentoAsync(estabelecimentoId, dataInicio, dataFim);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create([FromBody] CreatePedidoRequest request)
        {
            var result = await _pedidoService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.PedidoId }, result);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Profissional,Cliente")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusPedidoRequest request)
        {
            var result = await _pedidoService.UpdateStatusAsync(id, request);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _pedidoService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(result);
            
            return NoContent();
        }
    }

}
