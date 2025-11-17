using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.DTOs.Response.Endereco;
using Gemona.Infrastructure.ExternalServices.OpenCage.Models;
using Gemona.Infrastructure.ExternalServices.ViaCep;

namespace Gemona.Infrastructure.ExternalServices.OpenCage
{
    public class OpenCageGeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenCageGeocodingService> _logger;
        private readonly ViaCepService _viaCepService;
        private readonly string _apiKey;

        public OpenCageGeocodingService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<OpenCageGeocodingService> logger,
            ViaCepService viaCepService)
        {
            _httpClient = httpClient;
            _logger = logger;
            _viaCepService = viaCepService;
            _apiKey = configuration["OpenCage:ApiKey"] 
                ?? throw new InvalidOperationException("OpenCage ApiKey não configurada");
            
            _httpClient.BaseAddress = new Uri("https://api.opencagedata.com/");
        }

        public async Task<EnderecoResponse?> BuscarPorCepAsync(string cep)
        {
            try
            {
                _logger.LogInformation("Buscando CEP {Cep}", cep);

                // 1. Buscar endereço no ViaCEP (API brasileira, sem limite de requisições)
                var viaCepResult = await _viaCepService.BuscarCepAsync(cep);
                
                if (viaCepResult == null)
                {
                    _logger.LogWarning("CEP {Cep} não encontrado no ViaCEP", cep);
                    return null;
                }

                // 2. Montar endereço completo para buscar coordenadas no OpenCage
                var enderecoCompleto = $"{viaCepResult.Logradouro}, {viaCepResult.Bairro}, {viaCepResult.Localidade}, {viaCepResult.Uf}, Brazil";
                
                _logger.LogInformation("Buscando coordenadas para: {Endereco}", enderecoCompleto);

                // 3. Buscar coordenadas no OpenCage
                var coordenadas = await BuscarCoordenadasAsync(enderecoCompleto);

                if (coordenadas == null)
                {
                    _logger.LogWarning("Não foi possível obter coordenadas para o endereço: {Endereco}", enderecoCompleto);
                    
                    // Retorna dados do ViaCEP sem coordenadas
                    return new EnderecoResponse
                    {
                        Rua = viaCepResult.Logradouro,
                        Numero = "",
                        Complemento = viaCepResult.Complemento,
                        Bairro = viaCepResult.Bairro,
                        Cidade = viaCepResult.Localidade,
                        Estado = viaCepResult.Uf,
                        Cep = viaCepResult.Cep,
                        Latitude = 0,
                        Longitude = 0
                    };
                }

                // 4. Retornar dados combinados (ViaCEP + OpenCage)
                return new EnderecoResponse
                {
                    Rua = viaCepResult.Logradouro,
                    Numero = "",
                    Complemento = viaCepResult.Complemento,
                    Bairro = viaCepResult.Bairro,
                    Cidade = viaCepResult.Localidade,
                    Estado = viaCepResult.Uf,
                    Cep = viaCepResult.Cep,
                    Latitude = coordenadas.Value.Latitude ?? 0,
                    Longitude = coordenadas.Value.Longitude ?? 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar CEP {Cep}", cep);
                return null;
            }
        }

        public async Task<(decimal? Latitude, decimal? Longitude)?> BuscarCoordenadasAsync(string endereco)
        {
            try
            {
                var url = $"geocode/v1/json?q={Uri.EscapeDataString(endereco)}&key={_apiKey}&language=pt-BR&limit=1";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Erro ao buscar coordenadas para {Endereco}: {StatusCode}", endereco, response.StatusCode);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OpenCageResponse>(json);

                if (result?.Results == null || result.Results.Count == 0)
                {
                    _logger.LogWarning("Endereço não encontrado: {Endereco}", endereco);
                    return null;
                }

                var location = result.Results[0];
                return (location.Geometry.Lat, location.Geometry.Lng);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar coordenadas para {Endereco}", endereco);
                return null;
            }
        }
    }
}
