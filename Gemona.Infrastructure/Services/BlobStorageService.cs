using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gemona.Infrastructure.Services;

public interface IBlobStorageService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType);
    Task<bool> DeleteImageAsync(string blobName);
    Task<Stream> DownloadImageAsync(string blobName);
    Task<bool> BlobExistsAsync(string blobName);
    string GetBlobUrl(string blobName);
}

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<BlobStorageService> _logger;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger)
    {
        _logger = logger;
        
        var connectionString = configuration["AzureStorage:ConnectionString"] 
            ?? throw new InvalidOperationException("Azure Storage connection string não configurada");
        
        _containerName = configuration["AzureStorage:ContainerName"] 
            ?? throw new InvalidOperationException("Azure Storage container name não configurado");

        var blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
    }

    /// <summary>
    /// Faz upload de uma imagem para o Azure Blob Storage
    /// </summary>
    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType)
    {
        try
        {
            // Gerar nome único para o blob (evita conflitos)
            var blobName = $"{Guid.NewGuid()}_{fileName}";
            var blobClient = _containerClient.GetBlobClient(blobName);

            // Configurar headers HTTP
            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            // Upload do blob
            await blobClient.UploadAsync(imageStream, new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            });

            _logger.LogInformation("Imagem {FileName} enviada com sucesso como {BlobName}", fileName, blobName);

            return blobName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer upload da imagem {FileName}", fileName);
            throw;
        }
    }

    /// <summary>
    /// Deleta uma imagem do Azure Blob Storage
    /// </summary>
    public async Task<bool> DeleteImageAsync(string blobName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            var result = await blobClient.DeleteIfExistsAsync();

            if (result.Value)
            {
                _logger.LogInformation("Imagem {BlobName} deletada com sucesso", blobName);
            }
            else
            {
                _logger.LogWarning("Imagem {BlobName} não encontrada para deletar", blobName);
            }

            return result.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar imagem {BlobName}", blobName);
            throw;
        }
    }

    /// <summary>
    /// Baixa uma imagem do Azure Blob Storage
    /// </summary>
    public async Task<Stream> DownloadImageAsync(string blobName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            
            if (!await blobClient.ExistsAsync())
            {
                _logger.LogWarning("Imagem {BlobName} não encontrada", blobName);
                throw new FileNotFoundException($"Imagem {blobName} não encontrada");
            }

            var downloadResponse = await blobClient.DownloadAsync();
            _logger.LogInformation("Imagem {BlobName} baixada com sucesso", blobName);

            return downloadResponse.Value.Content;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao baixar imagem {BlobName}", blobName);
            throw;
        }
    }

    /// <summary>
    /// Verifica se um blob existe no storage
    /// </summary>
    public async Task<bool> BlobExistsAsync(string blobName)
    {
        try
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            return await blobClient.ExistsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência do blob {BlobName}", blobName);
            return false;
        }
    }

    /// <summary>
    /// Retorna a URL pública do blob
    /// </summary>
    public string GetBlobUrl(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        return blobClient.Uri.ToString();
    }
}
