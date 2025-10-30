using Gemona.Application.DTOs.Request.SubCategoria;
using Gemona.Application.DTOs.Response.SubCategoria;
using Gemona.Application.DTOs.Shared;

namespace Gemona.Application.Interfaces.Services
{
    public interface ISubCategoriaService
    {
        Task<ApiResponse<IEnumerable<SubCategoriaResponse>>> GetAllAsync();
        Task<ApiResponse<SubCategoriaResponse?>> GetByIdAsync(int id);
        Task<ApiResponse<SubCategoriaWithServicosResponse?>> GetSubCategoriaWithServicosAsync(int subCategoriaId);
        Task<ApiResponse<IEnumerable<SubCategoriaResponse>>> GetSubCategoriasByCategoriaAsync(int categoriaId);
        Task<ApiResponse<SubCategoriaResponse>> CreateAsync(CreateSubCategoriaRequest request);
        Task<ApiResponse<SubCategoriaResponse>> UpdateAsync(int id, UpdateSubCategoriaRequest request);
        Task<ApiResponse<bool>> DeleteAsync(int id);
        Task<ApiResponse<bool>> NomeExistsAsync(string nome, int categoriaId);
    }
}
