using Gemona.Application.DTOs.Response.Endereco;

namespace Gemona.Application.Interfaces.Services
{
    public interface IGeocodingService
    {
        /// <summary>
        /// Busca coordenadas e endereço completo por CEP
        /// </summary>
        Task<EnderecoResponse?> BuscarPorCepAsync(string cep);
        
        /// <summary>
        /// Busca coordenadas por endereço completo
        /// </summary>
        Task<(decimal? Latitude, decimal? Longitude)?> BuscarCoordenadasAsync(string endereco);
    }
}
