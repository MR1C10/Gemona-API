namespace Gemona.Application.DTOs.Shared
{
    public class ImageDownloadResult
    {
        public required Stream Content { get; set; }
        public required string ContentType { get; set; }
    }
}