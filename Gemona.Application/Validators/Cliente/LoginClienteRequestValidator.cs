using FluentValidation;
using Gemona.Application.DTOs.Request.Cliente;

namespace Gemona.Application.Validators.Cliente
{
    public class LoginClienteRequestValidator : AbstractValidator<LoginClienteRequest>
    {
        public LoginClienteRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");
        }
    }
}
