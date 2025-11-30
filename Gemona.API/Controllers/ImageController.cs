using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.Exceptions;
using System.Net.Mime;
using Gemona.Application.DTOs.Shared;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IBlobStorageService _blobStorageService;

        public ImageController(IBlobStorageService blobStorageService)
        {
            _blobStorageService = blobStorageService;
        }

        /// <summary>
        /// Baixa uma imagem do Azure Blob Storage
        /// </summary>
        /// <param name="blobName">Nome do blob da imagem</param>
        /// <returns>Arquivo da imagem</returns>
        [HttpGet("{blobName}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetImage(string blobName)
        {
            try
            {
                var result = await _blobStorageService.DownloadImageAsync(blobName);

                // Define o Content-Disposition para exibição inline
                Response.Headers.Append("Content-Disposition", new ContentDisposition { Inline = true, FileName = blobName }.ToString());

                return File(result.Content, result.ContentType);
            }
            catch (FileNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao baixar a imagem: {ex.Message}");
            }
        }

        /// <summary>
        /// Deleta uma imagem do Azure Blob Storage
        /// </summary>
        /// <param name="blobName">Nome do blob da imagem</param>
        /// <returns>Status da operação</returns>
        [HttpDelete("{blobName}")]
        [Authorize(Roles = "Admin,Profissional")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteImage(string blobName)
        {
            try
            {
                var deleted = await _blobStorageService.DeleteImageAsync(blobName);
                if (!deleted)
                {
                    return NotFound($"Imagem com o nome '{blobName}' não encontrada.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao deletar a imagem: {ex.Message}");
            }
        }
    }
}
