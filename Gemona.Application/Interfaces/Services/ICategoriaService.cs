using Gemona.Application.DTOs.Request.Categoria;
using Gemona.Application.DTOs.Response.Categoria;
using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.Interfaces.Services
{
    public interface ICategoriaService
    {
        Task<ApiResponse<IEnumerable<CategoriaResponse>>> GetAllAsync();
        Task<ApiResponse<CategoriaResponse?>> GetByIdAsync(int id);
        Task<ApiResponse<CategoriaWithSubCategoriasResponse?>> GetWithSubCategoriasAsync(int id);
        Task<ApiResponse<CategoriaResponse>> CreateAsync(CreateCategoriaRequest request);
        Task<ApiResponse<CategoriaResponse>> UpdateAsync(int id, UpdateCategoriaRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
    }
}