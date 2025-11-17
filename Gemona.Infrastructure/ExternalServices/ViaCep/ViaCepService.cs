using System.Text.Json;
using Microsoft.Extensions.Logging;
using Gemona.Infrastructure.ExternalServices.ViaCep.Models;

namespace Gemona.Infrastructure.ExternalServices.ViaCep
{
    public class ViaCepService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ViaCepService> _logger;

        public ViaCepService(
            HttpClient httpClient,
            ILogger<ViaCepService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri("https://viacep.com.br/");
        }

        public async Task<ViaCepResponse?> BuscarCepAsync(string cep)
        {
            try
            {
                // Remove formatação do CEP
                cep = cep.Replace("-", "").Replace(".", "").Trim();

                if (cep.Length != 8 || !cep.All(char.IsDigit))
                {
                    _logger.LogWarning("CEP inválido: {Cep}", cep);
                    return null;
                }

                var url = $"ws/{cep}/json/";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Erro ao buscar CEP {Cep} no ViaCEP: {StatusCode}", cep, response.StatusCode);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ViaCepResponse>(json, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                if (result?.Erro == true)
                {
                    _logger.LogWarning("CEP {Cep} não encontrado no ViaCEP", cep);
                    return null;
                }

                _logger.LogInformation("CEP {Cep} encontrado no ViaCEP: {Cidade}/{UF}", cep, result?.Localidade, result?.Uf);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar CEP {Cep} no ViaCEP", cep);
                return null;
            }
        }
    }
}
