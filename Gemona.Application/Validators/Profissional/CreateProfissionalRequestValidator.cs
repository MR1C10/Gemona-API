using FluentValidation;
using Gemona.Application.DTOs.Request.Profissional;

namespace Gemona.Application.Validators.Profissional
{
    public class CreateProfissionalRequestValidator : AbstractValidator<CreateProfissionalRequest>
    {
        public CreateProfissionalRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres")
                .Matches("^[a-zA-ZÀ-ÿ\\s]+$").WithMessage("Nome deve conter apenas letras");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .EmailAddress().WithMessage("Email inválido")
                .MaximumLength(256).WithMessage("Email deve ter no máximo 256 caracteres");

            RuleFor(x => x.Senha)
                .NotEmpty().WithMessage("Senha é obrigatória")
                .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres")
                .MaximumLength(100).WithMessage("Senha deve ter no máximo 100 caracteres")
                .Matches("[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula")
                .Matches("[a-z]").WithMessage("Senha deve conter pelo menos uma letra minúscula")
                .Matches("[0-9]").WithMessage("Senha deve conter pelo menos um número");

            RuleFor(x => x.Cpf)
                .NotEmpty().WithMessage("CPF é obrigatório")
                .Must(BeAValidCpf).WithMessage("CPF inválido");

            RuleFor(x => x.DataNascimento)
                .NotEmpty().WithMessage("Data de nascimento é obrigatória")
                .LessThan(DateTime.Now.AddYears(-18)).WithMessage("Profissional deve ter pelo menos 18 anos")
                .GreaterThan(DateTime.Now.AddYears(-120)).WithMessage("Data de nascimento inválida");

            RuleFor(x => x.Telefone)
                .NotEmpty().WithMessage("Telefone é obrigatório")
                .Matches("^\\(?\\d{2}\\)?[\\s-]?\\d{4,5}-?\\d{4}$")
                .WithMessage("Telefone inválido. Use formato: (11) 99999-9999");

            RuleFor(x => x.ImagemPerfilUrl)
                .MaximumLength(500).WithMessage("URL da imagem deve ter no máximo 500 caracteres")
                .Must(BeAValidUrl).When(x => !string.IsNullOrEmpty(x.ImagemPerfilUrl))
                .WithMessage("URL da imagem inválida");
        }

        private bool BeAValidCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;
            
            cpf = cpf.Replace(".", "").Replace("-", "").Replace(" ", "");
            
            if (cpf.Length != 11) return false;
            if (cpf.Distinct().Count() == 1) return false;

            var multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            var multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf.Substring(0, 9);
            var soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            var digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }

        private bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
