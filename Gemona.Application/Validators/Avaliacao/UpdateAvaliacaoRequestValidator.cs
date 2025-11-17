using FluentValidation;
using Gemona.Application.DTOs.Request.Avaliacao;
using Gemona.Domain.Enums;

namespace Gemona.Application.Validators.Avaliacao
{
    public class UpdateAvaliacaoRequestValidator : AbstractValidator<UpdateAvaliacaoRequest>
    {
        public UpdateAvaliacaoRequestValidator()
        {
            RuleFor(x => x.Nota)
                .NotEmpty().WithMessage("Nota é obrigatória")
                .IsInEnum().WithMessage("Nota inválida. Use: Pessimo, Ruim, Regular, Bom, Excelente");

            RuleFor(x => x.Comentario)
                .MinimumLength(10).WithMessage("Comentário deve ter no mínimo 10 caracteres")
                .MaximumLength(500).WithMessage("Comentário deve ter no máximo 500 caracteres")
                .When(x => !string.IsNullOrEmpty(x.Comentario));

            RuleFor(x => x.ImagemComentario)
                .Must(BeAValidBase64Image!).WithMessage("Imagem inválida")
                .When(x => x.ImagemComentario != null);
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
