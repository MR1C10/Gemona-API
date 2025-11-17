using FluentValidation;
using Gemona.Application.DTOs.Request.Servico;

namespace Gemona.Application.Validators.Servico
{
    public class UpdateServicoRequestValidator : AbstractValidator<UpdateServicoRequest>
    {
        public UpdateServicoRequestValidator()
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

            RuleFor(x => x.SubCategoriaId)
                .NotEmpty().WithMessage("SubCategoriaId é obrigatória")
                .GreaterThan(0).WithMessage("SubCategoriaId inválida");

            RuleFor(x => x.ImagemServico)
                .Must(BeAValidBase64Image!).WithMessage("Imagem inválida")
                .When(x => x.ImagemServico != null);
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
