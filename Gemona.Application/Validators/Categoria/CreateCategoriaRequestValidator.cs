using FluentValidation;
using Gemona.Application.DTOs.Request.Categoria;

namespace Gemona.Application.Validators.Categoria
{
    public class CreateCategoriaRequestValidator : AbstractValidator<CreateCategoriaRequest>
    {
        public CreateCategoriaRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
                .MaximumLength(50).WithMessage("Nome deve ter no máximo 50 caracteres")
                .Matches("^[a-zA-ZÀ-ÿ\\s]+$").WithMessage("Nome deve conter apenas letras");

            RuleFor(x => x.ImagemCategoriaUrl)
                .MaximumLength(500).WithMessage("URL da imagem deve ter no máximo 500 caracteres")
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.ImagemCategoriaUrl))
                .WithMessage("URL da imagem inválida");
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
