using Microsoft.AspNetCore.Http;

namespace Gemona.Application.DTOs.Request;

public class ImageUploadRequest
{
    public IFormFile? Image { get; set; }
}
