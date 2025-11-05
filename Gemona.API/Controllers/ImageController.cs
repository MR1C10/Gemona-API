using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gemona.Application.DTOs.Request;
using Gemona.Application.DTOs.Response;
using Gemona.Application.DTOs.Shared;
using Gemona.Infrastructure.Services;
using Gemona.Application.Exceptions;

namespace Gemona.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IBlobStorageService _blobStorageService;
    private readonly ILogger<ImageController> _logger;

    public ImageController(IBlobStorageService blobStorageService, ILogger<ImageController> logger)
    {
        _blobStorageService = blobStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Faz upload de uma imagem para o Azure Blob Storage
    /// </summary>
    /// <param name="request">Formulário com a imagem</param>
    /// <returns>Informações da imagem enviada</returns>
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<ImageUploadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ImageUploadResponse>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<ImageUploadResponse>>> UploadImage([FromForm] ImageUploadRequest request)
    {
        if (request.Image == null || request.Image.Length == 0)
        {
            throw new BusinessException("Nenhuma imagem foi enviada");
        }

        // Validar tamanho (5MB)
        const long maxFileSize = 5 * 1024 * 1024;
        if (request.Image.Length > maxFileSize)
        {
            throw new BusinessException($"A imagem excede o tamanho máximo permitido de {maxFileSize / 1024 / 1024}MB");
        }

        // Validar tipo de arquivo
        var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif" };
        if (!allowedContentTypes.Contains(request.Image.ContentType.ToLower()))
        {
            throw new BusinessException($"Tipo de arquivo não permitido. Tipos permitidos: {string.Join(", ", allowedContentTypes)}");
        }

        // Validar extensão
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var extension = Path.GetExtension(request.Image.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
        {
            throw new BusinessException($"Extensão de arquivo não permitida. Extensões permitidas: {string.Join(", ", allowedExtensions)}");
        }

        using var stream = request.Image.OpenReadStream();
        var blobName = await _blobStorageService.UploadImageAsync(
            stream,
            request.Image.FileName,
            request.Image.ContentType
        );

        var response = new ImageUploadResponse
        {
            BlobName = blobName,
            Url = _blobStorageService.GetBlobUrl(blobName),
            FileName = request.Image.FileName,
            Size = request.Image.Length,
            ContentType = request.Image.ContentType,
            UploadedAt = DateTime.UtcNow
        };

        _logger.LogInformation("Imagem {FileName} enviada com sucesso. BlobName: {BlobName}", 
            request.Image.FileName, blobName);

        return Ok(ApiResponse<ImageUploadResponse>.SuccessResult(response, "Imagem enviada com sucesso"));
    }

    /// <summary>
    /// Deleta uma imagem do Azure Blob Storage
    /// </summary>
    /// <param name="blobName">Nome do blob no storage</param>
    /// <returns>Confirmação de exclusão</returns>
    [HttpDelete("{blobName}")]
    [Authorize(Roles = "Admin,Profissional")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteImage(string blobName)
    {
        var exists = await _blobStorageService.BlobExistsAsync(blobName);
        if (!exists)
        {
            throw new NotFoundException("Imagem", blobName);
        }

        var deleted = await _blobStorageService.DeleteImageAsync(blobName);

        _logger.LogInformation("Imagem {BlobName} deletada com sucesso", blobName);

        return Ok(ApiResponse<bool>.SuccessResult(deleted, "Imagem deletada com sucesso"));
    }

    /// <summary>
    /// Baixa uma imagem do Azure Blob Storage
    /// </summary>
    /// <param name="blobName">Nome do blob no storage</param>
    /// <returns>Stream da imagem</returns>
    [HttpGet("{blobName}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadImage(string blobName)
    {
        var exists = await _blobStorageService.BlobExistsAsync(blobName);
        if (!exists)
        {
            throw new NotFoundException("Imagem", blobName);
        }

        var stream = await _blobStorageService.DownloadImageAsync(blobName);

        // Determinar o content type baseado na extensão
        var extension = Path.GetExtension(blobName).ToLower();
        var contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        _logger.LogInformation("Imagem {BlobName} baixada com sucesso", blobName);

        return File(stream, contentType);
    }

    /// <summary>
    /// Obtém a URL pública de uma imagem
    /// </summary>
    /// <param name="blobName">Nome do blob no storage</param>
    /// <returns>URL da imagem</returns>
    [HttpGet("{blobName}/url")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<string>>> GetImageUrl(string blobName)
    {
        var exists = await _blobStorageService.BlobExistsAsync(blobName);
        if (!exists)
        {
            throw new NotFoundException("Imagem", blobName);
        }

        var url = _blobStorageService.GetBlobUrl(blobName);

        return Ok(ApiResponse<string>.SuccessResult(url, "URL obtida com sucesso"));
    }
}
