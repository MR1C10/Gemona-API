namespace Gemona.Application.DTOs.Request.Cliente
{
    public class LoginClienteRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}