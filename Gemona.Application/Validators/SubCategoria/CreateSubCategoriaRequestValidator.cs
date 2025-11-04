using FluentValidation;
using Gemona.Application.DTOs.Request.SubCategoria;

namespace Gemona.Application.Validators.SubCategoria
{
    public class CreateSubCategoriaRequestValidator : AbstractValidator<CreateSubCategoriaRequest>
    {
        public CreateSubCategoriaRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
                .MaximumLength(50).WithMessage("Nome deve ter no máximo 50 caracteres");

            RuleFor(x => x.CategoriaId)
                .NotEmpty().WithMessage("CategoriaId é obrigatória")
                .GreaterThan(0).WithMessage("CategoriaId inválida");

            RuleFor(x => x.ImagemSubcategoriaUrl)
                .MaximumLength(500).WithMessage("URL da imagem deve ter no máximo 500 caracteres")
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.ImagemSubcategoriaUrl))
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
