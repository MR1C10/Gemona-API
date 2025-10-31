using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.DTOs.Request.Categoria;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoriaService.GetAllAsync();
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoriaService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("{id}/subcategorias")]
        public async Task<IActionResult> GetWithSubCategorias(int id)
        {
            var result = await _categoriaService.GetWithSubCategoriasAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoriaRequest request)
        {
            var result = await _categoriaService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.CategoriaId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaRequest request)
        {
            var result = await _categoriaService.UpdateAsync(id, request);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoriaService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(result);
            
            return NoContent();
        }
    }
}