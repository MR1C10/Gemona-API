using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.DTOs.Request.Profissional;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfissionalController : ControllerBase
    {
        private readonly IProfissionalService _profissionalService;

        public ProfissionalController(IProfissionalService profissionalService)
        {
            _profissionalService = profissionalService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _profissionalService.GetAllAsync();
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _profissionalService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var result = await _profissionalService.GetByEmailAsync(email);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("cpf/{cpf}")]
        public async Task<IActionResult> GetByCpf(string cpf)
        {
            var result = await _profissionalService.GetByCpfAsync(cpf);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("{id}/estabelecimento")]
        public async Task<IActionResult> GetWithEstabelecimento(int id)
        {
            var result = await _profissionalService.GetProfissionalWithEstabelecimentoAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateProfissionalRequest request)
        {
            var result = await _profissionalService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.ProfissionalId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProfissionalRequest request)
        {
            var result = await _profissionalService.UpdateAsync(id, request);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _profissionalService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(result);
            
            return NoContent();
        }

        [HttpGet("email-exists/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> EmailExists(string email)
        {
            var result = await _profissionalService.EmailExistsAsync(email);
            return Ok(result);
        }

        [HttpGet("cpf-exists/{cpf}")]
        [AllowAnonymous]
        public async Task<IActionResult> CpfExists(string cpf)
        {
            var result = await _profissionalService.CpfExistsAsync(cpf);
            return Ok(result);
        }
    }
}
