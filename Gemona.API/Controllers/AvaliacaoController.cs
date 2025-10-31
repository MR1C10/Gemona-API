using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.DTOs.Request.Avaliacao;
using Gemona.Domain.Enums;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AvaliacaoController : ControllerBase
    {
        private readonly IAvaliacaoService _avaliacaoService;

        public AvaliacaoController(IAvaliacaoService avaliacaoService)
        {
            _avaliacaoService = avaliacaoService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _avaliacaoService.GetAllAsync();
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _avaliacaoService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> GetByCliente(int clienteId)
        {
            var result = await _avaliacaoService.GetAvaliacoesByClienteAsync(clienteId);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("estabelecimento/{estabelecimentoId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByEstabelecimento(int estabelecimentoId)
        {
            var result = await _avaliacaoService.GetAvaliacoesByEstabelecimentoAsync(estabelecimentoId);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("nota/{nota}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByNota(NotaAvaliacao nota)
        {
            var result = await _avaliacaoService.GetAvaliacoesByNotaAsync(nota);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("estabelecimento/{estabelecimentoId}/media")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMediaEstabelecimento(int estabelecimentoId)
        {
            var result = await _avaliacaoService.GetMediaAvaliacoesEstabelecimentoAsync(estabelecimentoId);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPost("filtrar")]
        [AllowAnonymous]
        public async Task<IActionResult> FiltrarAvaliacoes([FromBody] FiltrarAvaliacoesRequest request)
        {
            var result = await _avaliacaoService.FiltrarAvaliacoesAsync(request);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create([FromBody] CreateAvaliacaoRequest request)
        {
            var result = await _avaliacaoService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.AvaliacaoId }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAvaliacaoRequest request)
        {
            var result = await _avaliacaoService.UpdateAsync(id, request);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Cliente,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _avaliacaoService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(result);
            
            return NoContent();
        }
    }
}
