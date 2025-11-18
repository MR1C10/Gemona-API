using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.DTOs.Request.Estabelecimento;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstabelecimentoController : ControllerBase
    {
        private readonly IEstabelecimentoService _estabelecimentoService;

        public EstabelecimentoController(IEstabelecimentoService estabelecimentoService)
        {
            _estabelecimentoService = estabelecimentoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _estabelecimentoService.GetAllAsync();
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _estabelecimentoService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("cnpj/{cnpj}")]
        public async Task<IActionResult> GetByCnpj(string cnpj)
        {
            var result = await _estabelecimentoService.GetByCnpjAsync(cnpj);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("{id}/completo")]
        public async Task<IActionResult> GetCompleto(int id)
        {
            var result = await _estabelecimentoService.GetEstabelecimentoCompletoAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("profissional/{profissionalId}")]
        public async Task<IActionResult> GetByProfissional(int profissionalId)
        {
            var result = await _estabelecimentoService.GetEstabelecimentosByProfissionalAsync(profissionalId);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("cidade/{cidade}")]
        public async Task<IActionResult> GetByCidade(string cidade)
        {
            var result = await _estabelecimentoService.GetEstabelecimentosByCidadeAsync(cidade);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("proximos")]
        public async Task<IActionResult> GetProximos([FromQuery] decimal latitude, [FromQuery] decimal longitude, [FromQuery] double raioKm = 10.0)
        {
            var result = await _estabelecimentoService.GetEstabelecimentosProximosAsync(latitude, longitude, raioKm);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string termo)
        {
            var result = await _estabelecimentoService.BuscarEstabelecimentosAsync(termo);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateEstabelecimentoRequest request)
        {
            var result = await _estabelecimentoService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.EstabelecimentoId }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEstabelecimentoRequest request)
        {
            var result = await _estabelecimentoService.UpdateAsync(id, request);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Profissional,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _estabelecimentoService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(result);
            
            return NoContent();
        }
    }
}
