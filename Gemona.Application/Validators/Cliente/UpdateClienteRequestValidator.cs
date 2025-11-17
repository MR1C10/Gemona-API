using FluentValidation;
using Gemona.Application.DTOs.Request.Cliente;

namespace Gemona.Application.Validators.Cliente
{
    public class UpdateClienteRequestValidator : AbstractValidator<UpdateClienteRequest>
    {
        public UpdateClienteRequestValidator()
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

        private bool BeAValidBase64Image(DTOs.Shared.Base64ImageDto? image)
        {
            if (image == null) return true;

            var validContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif" };
            if (!validContentTypes.Contains(image.ContentType?.ToLower()))
                return false;

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var fileExtension = System.IO.Path.GetExtension(image.FileName).ToLower();
            if (!validExtensions.Contains(fileExtension))
                return false;

            if (string.IsNullOrWhiteSpace(image.Base64Data))
                return false;

            try
            {
                var imageBytes = Convert.FromBase64String(image.Base64Data);
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
