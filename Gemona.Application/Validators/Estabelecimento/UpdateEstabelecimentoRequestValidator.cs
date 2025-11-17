using FluentValidation;
using Gemona.Application.DTOs.Request.Estabelecimento;

namespace Gemona.Application.Validators.Estabelecimento
{
    public class UpdateEstabelecimentoRequestValidator : AbstractValidator<UpdateEstabelecimentoRequest>
    {
        public UpdateEstabelecimentoRequestValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório")
                .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
                .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

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

            RuleFor(x => x.ImagemEstabelecimento)
                .Must(BeAValidBase64Image!).WithMessage("Imagem inválida")
                .When(x => x.ImagemEstabelecimento != null);
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
