using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.DTOs.Request.Servico;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicoController : ControllerBase
    {
        private readonly IServicoService _servicoService;

        public ServicoController(IServicoService servicoService)
        {
            _servicoService = servicoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _servicoService.GetAllAsync();
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _servicoService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("estabelecimento/{estabelecimentoId}")]
        public async Task<IActionResult> GetByEstabelecimento(int estabelecimentoId)
        {
            var result = await _servicoService.GetServicosByEstabelecimentoAsync(estabelecimentoId);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("categoria/{categoriaId}")]
        public async Task<IActionResult> GetByCategoria(int categoriaId)
        {
            var result = await _servicoService.GetServicosByCategoriaAsync(categoriaId);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("subcategoria/{subCategoriaId}")]
        public async Task<IActionResult> GetBySubCategoria(int subCategoriaId)
        {
            var result = await _servicoService.GetServicosBySubCategoriaAsync(subCategoriaId);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("preco")]
        public async Task<IActionResult> GetByFaixaPreco([FromQuery] decimal precoMinimo, [FromQuery] decimal precoMaximo)
        {
            var result = await _servicoService.GetServicosByFaixaPrecoAsync(precoMinimo, precoMaximo);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPost("buscar")]
        public async Task<IActionResult> BuscarServicos([FromBody] BuscarServicosRequest request)
        {
            var result = await _servicoService.BuscarServicosAsync(request);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Profissional")]
        public async Task<IActionResult> Create([FromBody] CreateServicoRequest request)
        {
            var result = await _servicoService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.ServicoId }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Profissional")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateServicoRequest request)
        {
            var result = await _servicoService.UpdateAsync(id, request);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Profissional,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _servicoService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(result);
            
            return NoContent();
        }
    }
}
