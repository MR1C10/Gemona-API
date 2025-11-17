using Microsoft.AspNetCore.Mvc;
using Gemona.Application.DTOs.Request.Endereco;
using Gemona.Application.DTOs.Response.Endereco;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Interfaces.Services;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnderecoController : ControllerBase
    {
        private readonly IGeocodingService _geocodingService;
        private readonly ILogger<EnderecoController> _logger;

        public EnderecoController(
            IGeocodingService geocodingService,
            ILogger<EnderecoController> logger)
        {
            _geocodingService = geocodingService;
            _logger = logger;
        }

        /// <summary>
        /// Busca dados completos do endereço por CEP (incluindo coordenadas)
        /// </summary>
        [HttpPost("buscar-por-cep")]
        [ProducesResponseType(typeof(ApiResponse<EnderecoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<EnderecoResponse>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<EnderecoResponse>>> BuscarPorCep([FromBody] BuscarPorCepRequest request)
        {
            var resultado = await _geocodingService.BuscarPorCepAsync(request.Cep);

            if (resultado == null)
            {
                return NotFound(ApiResponse<EnderecoResponse>.ErrorResult("CEP não encontrado"));
            }

            return Ok(ApiResponse<EnderecoResponse>.SuccessResult(resultado, "Endereço encontrado com sucesso"));
        }
    }
}
