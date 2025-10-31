using Gemona.Application.DTOs.Request.Categoria;
using Gemona.Application.DTOs.Response.Categoria;
using Gemona.Application.DTOs.Response.SubCategoria;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;

namespace Gemona.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }

        public async Task<ApiResponse<IEnumerable<CategoriaResponse>>> GetAllAsync()
        {
            try
            {
                var categorias = await _categoriaRepository.GetAllActiveAsync();
                var response = categorias.Select(MapToResponse);
                
                return ApiResponse<IEnumerable<CategoriaResponse>>.SuccessResult(
                    response, "Categorias recuperadas com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<CategoriaResponse>>.ErrorResult(
                    "Erro ao buscar categorias", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CategoriaResponse?>> GetByIdAsync(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    return ApiResponse<CategoriaResponse?>.ErrorResult("Categoria não encontrada");
                }

                var response = MapToResponse(categoria);
                return ApiResponse<CategoriaResponse?>.SuccessResult(
                    response, "Categoria encontrada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoriaResponse?>.ErrorResult(
                    "Erro ao buscar categoria", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CategoriaWithSubCategoriasResponse?>> GetWithSubCategoriasAsync(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetCategoriaWithSubCategoriasAsync(id);
                if (categoria == null)
                {
                    return ApiResponse<CategoriaWithSubCategoriasResponse?>.ErrorResult("Categoria não encontrada");
                }

                var response = MapToResponseWithSubCategorias(categoria);
                return ApiResponse<CategoriaWithSubCategoriasResponse?>.SuccessResult(
                    response, "Categoria com subcategorias encontrada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoriaWithSubCategoriasResponse?>.ErrorResult(
                    "Erro ao buscar categoria", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CategoriaResponse>> CreateAsync(CreateCategoriaRequest request)
        {
            try
            {
                // Validar se nome já existe
                if (await _categoriaRepository.NomeExistsAsync(request.Nome))
                {
                    return ApiResponse<CategoriaResponse>.ErrorResult("Já existe uma categoria com este nome");
                }

                var categoria = new Categoria
                {
                    Nome = request.Nome,
                    ImagemCategoriaUrl = request.ImagemCategoriaUrl
                };

                var result = await _categoriaRepository.AddAsync(categoria);
                await _categoriaRepository.SaveChangesAsync();

                var response = MapToResponse(result);
                return ApiResponse<CategoriaResponse>.SuccessResult(
                    response, "Categoria criada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoriaResponse>.ErrorResult(
                    "Erro ao criar categoria", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CategoriaResponse>> UpdateAsync(int id, UpdateCategoriaRequest request)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    return ApiResponse<CategoriaResponse>.ErrorResult("Categoria não encontrada");
                }

                // Verificar se novo nome já existe (exceto na própria categoria)
                var categoriaExistente = await _categoriaRepository.GetByNomeAsync(request.Nome);
                if (categoriaExistente != null && categoriaExistente.CategoriaId != id)
                {
                    return ApiResponse<CategoriaResponse>.ErrorResult("Já existe uma categoria com este nome");
                }

                categoria.Nome = request.Nome;
                categoria.ImagemCategoriaUrl = request.ImagemCategoriaUrl;
                categoria.DataAtualizacao = DateTime.UtcNow;

                await _categoriaRepository.UpdateAsync(categoria);
                await _categoriaRepository.SaveChangesAsync();

                var response = MapToResponse(categoria);
                return ApiResponse<CategoriaResponse>.SuccessResult(
                    response, "Categoria atualizada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<CategoriaResponse>.ErrorResult(
                    "Erro ao atualizar categoria", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var categoria = await _categoriaRepository.GetByIdAsync(id);
                if (categoria == null)
                {
                    return ApiResponse<bool>.ErrorResult("Categoria não encontrada");
                }

                await _categoriaRepository.DeleteAsync(id);
                await _categoriaRepository.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Categoria excluída com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResult(
                    "Erro ao excluir categoria", new List<string> { ex.Message });
            }
        }

        // Métodos de mapeamento
        private static CategoriaResponse MapToResponse(Categoria categoria)
        {
            return new CategoriaResponse
            {
                CategoriaId = categoria.CategoriaId,
                Nome = categoria.Nome,
                ImagemCategoriaUrl = categoria.ImagemCategoriaUrl,
                DataCriacao = categoria.DataCriacao,
                DataAtualizacao = categoria.DataAtualizacao,
                Ativo = categoria.Ativo
            };
        }

        private static CategoriaWithSubCategoriasResponse MapToResponseWithSubCategorias(Categoria categoria)
        {
            return new CategoriaWithSubCategoriasResponse
            {
                CategoriaId = categoria.CategoriaId,
                Nome = categoria.Nome,
                ImagemCategoriaUrl = categoria.ImagemCategoriaUrl,
                DataCriacao = categoria.DataCriacao,
                DataAtualizacao = categoria.DataAtualizacao,
                Ativo = categoria.Ativo,
                SubCategorias = categoria.SubCategorias?.Select(sc => new SubCategoriaResponse
                {
                    SubCategoriaId = sc.SubCategoriaId,
                    Nome = sc.Nome,
                    CategoriaId = sc.CategoriaId,
                    CategoriaNome = sc.Categoria?.Nome ?? "",
                    ImagemSubcategoriaUrl = sc.ImagemSubcategoriaUrl,
                    DataCriacao = sc.DataCriacao,
                    DataAtualizacao = sc.DataAtualizacao,
                    Ativo = sc.Ativo
                }) ?? new List<SubCategoriaResponse>()
            };
        }
    }
}