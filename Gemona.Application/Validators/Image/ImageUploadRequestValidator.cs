using FluentValidation;
using Gemona.Application.DTOs.Request;

namespace Gemona.Application.Validators.Image;

public class ImageUploadRequestValidator : AbstractValidator<ImageUploadRequest>
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
    private static readonly string[] AllowedContentTypes = {
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp",
        "image/gif"
    };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public ImageUploadRequestValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("A imagem é obrigatória");

        When(x => x.Image != null, () =>
        {
            RuleFor(x => x.Image!.Length)
                .LessThanOrEqualTo(MaxFileSize)
                .WithMessage($"O tamanho máximo permitido é {MaxFileSize / 1024 / 1024}MB");

            RuleFor(x => x.Image!.Length)
                .GreaterThan(0)
                .WithMessage("A imagem não pode estar vazia");

            RuleFor(x => x.Image!.ContentType)
                .Must(contentType => AllowedContentTypes.Contains(contentType.ToLower()))
                .WithMessage($"Tipo de arquivo não permitido. Tipos permitidos: {string.Join(", ", AllowedContentTypes)}");

            RuleFor(x => x.Image!.FileName)
                .Must(fileName =>
                {
                    var extension = Path.GetExtension(fileName).ToLower();
                    return AllowedExtensions.Contains(extension);
                })
                .WithMessage($"Extensão de arquivo não permitida. Extensões permitidas: {string.Join(", ", AllowedExtensions)}");
        });
    }
}
