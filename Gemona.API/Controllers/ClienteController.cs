using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.DTOs.Request.Cliente;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _clienteService.GetAllAsync();
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _clienteService.GetByIdAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var result = await _clienteService.GetByEmailAsync(email);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("cpf/{cpf}")]
        public async Task<IActionResult> GetByCpf(string cpf)
        {
            var result = await _clienteService.GetByCpfAsync(cpf);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpGet("{id}/endereco")]
        public async Task<IActionResult> GetWithEndereco(int id)
        {
            var result = await _clienteService.GetClienteWithEnderecoAsync(id);
            if (!result.Success)
                return NotFound(result);
            
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateClienteRequest request)
        {
            var result = await _clienteService.CreateAsync(request);
            if (!result.Success)
                return BadRequest(result);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.ClienteId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateClienteRequest request)
        {
            var result = await _clienteService.UpdateAsync(id, request);
            if (!result.Success)
                return BadRequest(result);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _clienteService.DeleteAsync(id);
            if (!result.Success)
                return BadRequest(result);
            
            return NoContent();
        }

        [HttpGet("email-exists/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> EmailExists(string email)
        {
            var result = await _clienteService.EmailExistsAsync(email);
            return Ok(result);
        }

        [HttpGet("cpf-exists/{cpf}")]
        [AllowAnonymous]
        public async Task<IActionResult> CpfExists(string cpf)
        {
            var result = await _clienteService.CpfExistsAsync(cpf);
            return Ok(result);
        }
    }
}
