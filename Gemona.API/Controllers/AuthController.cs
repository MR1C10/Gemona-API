using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.DTOs.Request.Cliente;
using Gemona.Application.DTOs.Request.Profissional;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login/cliente")]
        public async Task<IActionResult> LoginCliente([FromBody] LoginClienteRequest request)
        {
            var result = await _authService.LoginClienteAsync(request);
            if (!result.Success)
                return Unauthorized(result);
            
            return Ok(result);
        }

        [HttpPost("login/profissional")]
        public async Task<IActionResult> LoginProfissional([FromBody] LoginProfissionalRequest request)
        {
            var result = await _authService.LoginProfissionalAsync(request);
            if (!result.Success)
                return Unauthorized(result);
            
            return Ok(result);
        }

        [HttpPost("login/admin")]
        public async Task<IActionResult> LoginAdmin([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAdminAsync(request.Email, request.Senha);
            if (!result.Success)
                return Unauthorized(result);
            
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.Email, request.Senha);
            if (!result.Success)
                return Unauthorized(result);
            
            return Ok(result);
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenRequest request)
        {
            var result = await _authService.ValidateTokenAsync(request.Token);
            if (!result.Success)
                return Unauthorized(result);
            
            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RefreshTokenAsync(request.Token);
            if (!result.Success)
                return Unauthorized(result);
            
            return Ok(result);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class ValidateTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
