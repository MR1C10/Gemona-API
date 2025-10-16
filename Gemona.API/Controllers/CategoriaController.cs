using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Domain.Entities;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaController(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var categorias = await _categoriaRepository.GetAllActiveAsync();
                return Ok(categorias);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    return NotFound(new { message = "Categoria não encontrada" });
                }

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/subcategorias")]
        public async Task<IActionResult> GetWithSubCategorias(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetCategoriaWithSubCategoriasAsync(id);
                if (categoria == null)
                {
                    return NotFound(new { message = "Categoria não encontrada" });
                }

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoriaRequest request)
        {
            try
            {
                // Verificar se nome já existe
                if (await _categoriaRepository.NomeExistsAsync(request.Nome))
                {
                    return BadRequest(new { message = "Já existe uma categoria com este nome" });
                }

                var categoria = new Categoria
                {
                    Nome = request.Nome,
                    ImagemCategoriaUrl = request.ImagemCategoriaUrl
                };

                var resultado = await _categoriaRepository.AddAsync(categoria);
                await _categoriaRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = resultado.CategoriaId }, resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoriaRequest request)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    return NotFound(new { message = "Categoria não encontrada" });
                }
                // Verificar se novo nome já existe (exceto na própria categoria)
                var categoriaExistente = await _categoriaRepository.GetByNomeAsync(request.Nome);
                if (categoriaExistente != null && categoriaExistente.CategoriaId != id)
                {
                    return BadRequest(new { message = "Já existe uma categoria com este nome" });
                }

                categoria.Nome = request.Nome;
                categoria.ImagemCategoriaUrl = request.ImagemCategoriaUrl;
                categoria.DataAtualizacao = DateTime.UtcNow;

                await _categoriaRepository.UpdateAsync(categoria);
                await _categoriaRepository.SaveChangesAsync();

                return Ok(categoria);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    return NotFound(new { message = "Categoria não encontrada" });
                }

                await _categoriaRepository.DeleteAsync(id);
                await _categoriaRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    // DTOs simples para requests
    public class CreateCategoriaRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? ImagemCategoriaUrl { get; set; }
    }

    public class UpdateCategoriaRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string? ImagemCategoriaUrl { get; set; }
    }
}