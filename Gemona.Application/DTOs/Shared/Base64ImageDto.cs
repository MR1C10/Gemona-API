namespace Gemona.Application.DTOs.Shared;

public class Base64ImageDto
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Base64Data { get; set; } = string.Empty;
}
