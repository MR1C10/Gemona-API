using FluentValidation;
using Gemona.Application.DTOs.Request.Profissional;

namespace Gemona.Application.Validators.Profissional
{
    public class LoginProfissionalRequestValidator : AbstractValidator<LoginProfissionalRequest>
    {
        public LoginProfissionalRequestValidator()
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
