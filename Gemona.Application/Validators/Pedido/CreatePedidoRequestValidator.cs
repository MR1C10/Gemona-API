using FluentValidation;
using Gemona.Application.DTOs.Request.Pedido;

namespace Gemona.Application.Validators.Pedido
{
    public class CreatePedidoRequestValidator : AbstractValidator<CreatePedidoRequest>
    {
        public CreatePedidoRequestValidator()
        {
            RuleFor(x => x.ClienteId)
                .NotEmpty().WithMessage("ClienteId é obrigatório")
                .GreaterThan(0).WithMessage("ClienteId inválido");

            RuleFor(x => x.ServicoId)
                .NotEmpty().WithMessage("ServicoId é obrigatório")
                .GreaterThan(0).WithMessage("ServicoId inválido");

            RuleFor(x => x.DataAgendamento)
                .Must(date => date == null || date >= DateTime.Now)
                .WithMessage("Data de agendamento deve ser futura");

            RuleFor(x => x.Observacoes)
                .MaximumLength(500).WithMessage("Observações devem ter no máximo 500 caracteres");
        }
    }
}
