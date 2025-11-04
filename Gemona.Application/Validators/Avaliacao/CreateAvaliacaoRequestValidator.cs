using FluentValidation;
using Gemona.Application.DTOs.Request.Avaliacao;
using Gemona.Domain.Enums;

namespace Gemona.Application.Validators.Avaliacao
{
    public class CreateAvaliacaoRequestValidator : AbstractValidator<CreateAvaliacaoRequest>
    {
        public CreateAvaliacaoRequestValidator()
        {
            RuleFor(x => x.ClienteId)
                .NotEmpty().WithMessage("ClienteId é obrigatório")
                .GreaterThan(0).WithMessage("ClienteId inválido");

            RuleFor(x => x.PedidoId)
                .NotEmpty().WithMessage("PedidoId é obrigatório")
                .GreaterThan(0).WithMessage("PedidoId inválido");

            RuleFor(x => x.Nota)
                .NotEmpty().WithMessage("Nota é obrigatória")
                .IsInEnum().WithMessage("Nota inválida. Use: Pessimo, Ruim, Regular, Bom, Excelente");

            RuleFor(x => x.Comentario)
                .MinimumLength(10).WithMessage("Comentário deve ter no mínimo 10 caracteres")
                .MaximumLength(500).WithMessage("Comentário deve ter no máximo 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Comentario));
        }
    }
}
