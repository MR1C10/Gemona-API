using FluentValidation;
using Gemona.Application.DTOs.Request.Servico;

namespace Gemona.Application.Validators.Servico
{
    public class CreateServicoRequestValidator : AbstractValidator<CreateServicoRequest>
    {
        public CreateServicoRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

            RuleFor(x => x.Descricao)
                .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");

            RuleFor(x => x.Preco)
                .NotEmpty().WithMessage("Preço é obrigatório")
                .GreaterThan(0).WithMessage("Preço deve ser maior que zero")
                .LessThanOrEqualTo(999999.99m).WithMessage("Preço deve ser menor que 999.999,99");

            RuleFor(x => x.EstabelecimentoId)
                .NotEmpty().WithMessage("EstabelecimentoId é obrigatório")
                .GreaterThan(0).WithMessage("EstabelecimentoId inválido");

            RuleFor(x => x.SubCategoriaId)
                .NotEmpty().WithMessage("SubCategoriaId é obrigatória")
                .GreaterThan(0).WithMessage("SubCategoriaId inválida");

            RuleFor(x => x.ImagemServicoUrl)
                .MaximumLength(500).WithMessage("URL da imagem deve ter no máximo 500 caracteres")
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.ImagemServicoUrl))
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
