using FluentValidation;
using Gemona.Application.DTOs.Request.Estabelecimento;

namespace Gemona.Application.Validators.Estabelecimento
{
    public class CreateEstabelecimentoRequestValidator : AbstractValidator<CreateEstabelecimentoRequest>
    {
        public CreateEstabelecimentoRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

            RuleFor(x => x.Cnpj)
                .NotEmpty().WithMessage("CNPJ é obrigatório")
                .Must(BeAValidCnpj).WithMessage("CNPJ inválido");

            RuleFor(x => x.Telefone)
                .NotEmpty().WithMessage("Telefone é obrigatório")
                .Matches("^\\(?\\d{2}\\)?[\\s-]?\\d{4,5}-?\\d{4}$")
                .WithMessage("Telefone inválido. Use formato: (11) 99999-9999");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(256).WithMessage("Email deve ter no máximo 256 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.Descricao)
                .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres");

            RuleFor(x => x.ImagemEstabelecimentoUrl)
                .MaximumLength(500).WithMessage("URL da imagem deve ter no máximo 500 caracteres")
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.ImagemEstabelecimentoUrl))
                .WithMessage("URL da imagem inválida");

            RuleFor(x => x.ProfissionalId)
                .GreaterThan(0).WithMessage("ProfissionalId inválido");

            // Validação de Endereço
            RuleFor(x => x.Rua)
                .NotEmpty().WithMessage("Rua é obrigatória")
                .MaximumLength(200).WithMessage("Rua deve ter no máximo 200 caracteres");

            RuleFor(x => x.Numero)
                .MaximumLength(10).WithMessage("Número deve ter no máximo 10 caracteres");

            RuleFor(x => x.Bairro)
                .NotEmpty().WithMessage("Bairro é obrigatório")
                .MaximumLength(100).WithMessage("Bairro deve ter no máximo 100 caracteres");

            RuleFor(x => x.Cidade)
                .NotEmpty().WithMessage("Cidade é obrigatória")
                .MaximumLength(100).WithMessage("Cidade deve ter no máximo 100 caracteres");

            RuleFor(x => x.Estado)
                .NotEmpty().WithMessage("Estado é obrigatório")
                .Length(2).WithMessage("Estado deve ter 2 caracteres (UF)")
                .Matches("^[A-Z]{2}$").WithMessage("Estado deve ser uma UF válida (ex: SP, RJ)");

            RuleFor(x => x.Cep)
                .NotEmpty().WithMessage("CEP é obrigatório")
                .Matches("^\\d{5}-?\\d{3}$").WithMessage("CEP inválido. Use formato: 12345-678");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90).WithMessage("Latitude deve estar entre -90 e 90");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180).WithMessage("Longitude deve estar entre -180 e 180");
        }

        private bool BeAValidCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj)) return false;
            
            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Replace(" ", "");
            
            if (cnpj.Length != 14) return false;
            if (cnpj.Distinct().Count() == 1) return false;

            var multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCnpj = cnpj.Substring(0, 12);
            var soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            var resto = (soma % 11);
            resto = resto < 2 ? 0 : 11 - resto;

            var digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = (soma % 11);
            resto = resto < 2 ? 0 : 11 - resto;
            digito = digito + resto.ToString();

            return cnpj.EndsWith(digito);
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
