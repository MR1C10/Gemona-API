using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.DTOs.Request.SubCategoria;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoriaController : ControllerBase
    {
        private readonly ISubCategoriaService _subCategoriaService;

        public SubCategoriaController(ISubCategoriaService subCategoriaService)
        {
            _subCategoriaService = subCategoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _subCategoriaService.GetAllAsync();
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _subCategoriaService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("{id}/servicos")]
        public async Task<IActionResult> GetWithServicos(int id)
        {
            var result = await _subCategoriaService.GetSubCategoriaWithServicosAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("categoria/{categoriaId}")]
        public async Task<IActionResult> GetByCategoria(int categoriaId)
        {
            var result = await _subCategoriaService.GetSubCategoriasByCategoriaAsync(categoriaId);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateSubCategoriaRequest request)
        {
            var result = await _subCategoriaService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.SubCategoriaId }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSubCategoriaRequest request)
        {
            var result = await _subCategoriaService.UpdateAsync(id, request);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _subCategoriaService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(result);
            
            return NoContent();
        }
    }
}
