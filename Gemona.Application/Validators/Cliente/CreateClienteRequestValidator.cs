using FluentValidation;
using Gemona.Application.DTOs.Request.Cliente;

namespace Gemona.Application.Validators.Cliente
{
    public class CreateClienteRequestValidator : AbstractValidator<CreateClienteRequest>
    {
        public CreateClienteRequestValidator()
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
                .LessThan(DateTime.Now.AddYears(-18)).WithMessage("Cliente deve ter pelo menos 18 anos")
                .GreaterThan(DateTime.Now.AddYears(-120)).WithMessage("Data de nascimento inválida");

            RuleFor(x => x.Telefone)
                .NotEmpty().WithMessage("Telefone é obrigatório")
                .Matches("^\\(?\\d{2}\\)?[\\s-]?\\d{4,5}-?\\d{4}$")
                .WithMessage("Telefone inválido. Use formato: (11) 99999-9999");

            RuleFor(x => x.ImagemPerfil)
                .Must(BeAValidBase64Image!).WithMessage("Imagem inválida")
                .When(x => x.ImagemPerfil != null);
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

        private bool BeAValidBase64Image(DTOs.Shared.Base64ImageDto? image)
        {
            if (image == null) return true;

            // Validar FileName
            if (string.IsNullOrWhiteSpace(image.FileName))
                return false;

            // Validar ContentType
            var validContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif" };
            if (!validContentTypes.Contains(image.ContentType?.ToLower()))
                return false;

            // Validar extensão do arquivo
            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var fileExtension = System.IO.Path.GetExtension(image.FileName).ToLower();
            if (!validExtensions.Contains(fileExtension))
                return false;

            // Validar Base64Data
            if (string.IsNullOrWhiteSpace(image.Base64Data))
                return false;

            try
            {
                // Tentar decodificar Base64
                var imageBytes = Convert.FromBase64String(image.Base64Data);

                // Validar tamanho (máximo 5MB)
                const int maxSizeInBytes = 5 * 1024 * 1024;
                if (imageBytes.Length > maxSizeInBytes)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
