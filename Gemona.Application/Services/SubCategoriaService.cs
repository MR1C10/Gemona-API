using Gemona.Application.DTOs.Request.SubCategoria;
using Gemona.Application.DTOs.Response.SubCategoria;
using Gemona.Application.DTOs.Response.Servico;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;

namespace Gemona.Application.Services
{
    public class SubCategoriaService : ISubCategoriaService
    {
        private readonly ISubCategoriaRepository _subCategoriaRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public SubCategoriaService(
            ISubCategoriaRepository subCategoriaRepository,
            ICategoriaRepository categoriaRepository)
        {
            _subCategoriaRepository = subCategoriaRepository;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<ApiResponse<IEnumerable<SubCategoriaResponse>>> GetAllAsync()
        {
            try
            {
                var subCategorias = await _subCategoriaRepository.GetAllActiveAsync();
                var response = subCategorias.Select(MapToResponse);
                
                return ApiResponse<IEnumerable<SubCategoriaResponse>>.SuccessResult(
                    response, "Subcategorias recuperadas com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<SubCategoriaResponse>>.ErrorResult(
                    "Erro ao buscar subcategorias", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<SubCategoriaResponse?>> GetByIdAsync(int id)
        {
            try
            {
                var subCategoria = await _subCategoriaRepository.GetByIdAsync(id);
                if (subCategoria == null)
                {
                    return ApiResponse<SubCategoriaResponse?>.ErrorResult("Subcategoria não encontrada");
                }

                var response = MapToResponse(subCategoria);
                return ApiResponse<SubCategoriaResponse?>.SuccessResult(
                    response, "Subcategoria encontrada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<SubCategoriaResponse?>.ErrorResult(
                    "Erro ao buscar subcategoria", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<SubCategoriaWithServicosResponse?>> GetSubCategoriaWithServicosAsync(int subCategoriaId)
        {
            try
            {
                var subCategoria = await _subCategoriaRepository.GetSubCategoriaWithServicosAsync(subCategoriaId);
                if (subCategoria == null)
                {
                    return ApiResponse<SubCategoriaWithServicosResponse?>.ErrorResult("Subcategoria não encontrada");
                }

                var response = MapToResponseWithServicos(subCategoria);
                return ApiResponse<SubCategoriaWithServicosResponse?>.SuccessResult(
                    response, "Subcategoria com serviços encontrada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<SubCategoriaWithServicosResponse?>.ErrorResult(
                    "Erro ao buscar subcategoria", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<IEnumerable<SubCategoriaResponse>>> GetSubCategoriasByCategoriaAsync(int categoriaId)
        {
            try
            {
                var subCategorias = await _subCategoriaRepository.GetSubCategoriasByCategoriaAsync(categoriaId);
                var response = subCategorias.Select(MapToResponse);
                
                return ApiResponse<IEnumerable<SubCategoriaResponse>>.SuccessResult(
                    response, "Subcategorias da categoria encontradas com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<SubCategoriaResponse>>.ErrorResult(
                    "Erro ao buscar subcategorias por categoria", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<SubCategoriaResponse>> CreateAsync(CreateSubCategoriaRequest request)
        {
            try
            {
                // Verificar se categoria existe
                var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId);
                if (categoria == null)
                {
                    return ApiResponse<SubCategoriaResponse>.ErrorResult("Categoria não encontrada");
                }

                // Validar se nome já existe na categoria
                if (await _subCategoriaRepository.NomeExistsAsync(request.Nome, request.CategoriaId))
                {
                    return ApiResponse<SubCategoriaResponse>.ErrorResult(
                        "Já existe uma subcategoria com este nome nesta categoria");
                }

                var subCategoria = new SubCategoria
                {
                    Nome = request.Nome,
                    CategoriaId = request.CategoriaId,
                    ImagemSubcategoriaUrl = request.ImagemSubcategoriaUrl
                };

                var result = await _subCategoriaRepository.AddAsync(subCategoria);
                await _subCategoriaRepository.SaveChangesAsync();

                var response = MapToResponse(result);
                return ApiResponse<SubCategoriaResponse>.SuccessResult(
                    response, "Subcategoria criada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<SubCategoriaResponse>.ErrorResult(
                    "Erro ao criar subcategoria", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<SubCategoriaResponse>> UpdateAsync(int id, UpdateSubCategoriaRequest request)
        {
            try
            {
                var subCategoria = await _subCategoriaRepository.GetByIdAsync(id);
                if (subCategoria == null)
                {
                    return ApiResponse<SubCategoriaResponse>.ErrorResult("Subcategoria não encontrada");
                }

                // Verificar se nova categoria existe
                var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId);
                if (categoria == null)
                {
                    return ApiResponse<SubCategoriaResponse>.ErrorResult("Categoria não encontrada");
                }

                // Verificar se novo nome já existe na categoria (exceto na própria subcategoria)
                var subCategoriaExistente = await _subCategoriaRepository.GetByNomeAsync(request.Nome);
                if (subCategoriaExistente != null && 
                    subCategoriaExistente.SubCategoriaId != id && 
                    subCategoriaExistente.CategoriaId == request.CategoriaId)
                {
                    return ApiResponse<SubCategoriaResponse>.ErrorResult(
                        "Já existe uma subcategoria com este nome nesta categoria");
                }

                subCategoria.Nome = request.Nome;
                subCategoria.CategoriaId = request.CategoriaId;
                subCategoria.ImagemSubcategoriaUrl = request.ImagemSubcategoriaUrl;
                subCategoria.DataAtualizacao = DateTime.UtcNow;

                await _subCategoriaRepository.UpdateAsync(subCategoria);
                await _subCategoriaRepository.SaveChangesAsync();

                var response = MapToResponse(subCategoria);
                return ApiResponse<SubCategoriaResponse>.SuccessResult(
                    response, "Subcategoria atualizada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<SubCategoriaResponse>.ErrorResult(
                    "Erro ao atualizar subcategoria", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var subCategoria = await _subCategoriaRepository.GetByIdAsync(id);
                if (subCategoria == null)
                {
                    return ApiResponse<bool>.ErrorResult("Subcategoria não encontrada");
                }

                await _subCategoriaRepository.DeleteAsync(id);
                await _subCategoriaRepository.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Subcategoria excluída com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResult(
                    "Erro ao excluir subcategoria", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> NomeExistsAsync(string nome, int categoriaId)
        {
            try
            {
                var exists = await _subCategoriaRepository.NomeExistsAsync(nome, categoriaId);
                return ApiResponse<bool>.SuccessResult(exists, "Verificação realizada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResult(
                    "Erro ao verificar nome", new List<string> { ex.Message });
            }
        }

        // Métodos de mapeamento
        private static SubCategoriaResponse MapToResponse(SubCategoria subCategoria)
        {
            return new SubCategoriaResponse
            {
                SubCategoriaId = subCategoria.SubCategoriaId,
                Nome = subCategoria.Nome,
                CategoriaId = subCategoria.CategoriaId,
                CategoriaNome = subCategoria.Categoria?.Nome ?? string.Empty,
                ImagemSubcategoriaUrl = subCategoria.ImagemSubcategoriaUrl,
                DataCriacao = subCategoria.DataCriacao,
                DataAtualizacao = subCategoria.DataAtualizacao,
                Ativo = subCategoria.Ativo
            };
        }

        private static SubCategoriaWithServicosResponse MapToResponseWithServicos(SubCategoria subCategoria)
        {
            return new SubCategoriaWithServicosResponse
            {
                SubCategoriaId = subCategoria.SubCategoriaId,
                Nome = subCategoria.Nome,
                CategoriaId = subCategoria.CategoriaId,
                CategoriaNome = subCategoria.Categoria?.Nome ?? string.Empty,
                ImagemSubcategoriaUrl = subCategoria.ImagemSubcategoriaUrl,
                DataCriacao = subCategoria.DataCriacao,
                DataAtualizacao = subCategoria.DataAtualizacao,
                Ativo = subCategoria.Ativo,
                Servicos = subCategoria.Servicos?.Select(s => new ServicoResponse
                {
                    ServicoId = s.ServicoId,
                    Nome = s.Nome,
                    Descricao = s.Descricao,
                    SubCategoriaId = s.SubCategoriaId,
                    SubCategoriaNome = s.SubCategoria?.Nome ?? string.Empty,
                    CategoriaNome = s.SubCategoria?.Categoria?.Nome ?? string.Empty,
                    Preco = s.Preco,
                    ImagemServicoUrl = s.ImagemServicoUrl,
                    EstabelecimentoId = s.EstabelecimentoId,
                    EstabelecimentoNome = s.Estabelecimento?.Nome ?? string.Empty,
                    DataCriacao = s.DataCriacao,
                    DataAtualizacao = s.DataAtualizacao,
                    Ativo = s.Ativo
                }) ?? new List<ServicoResponse>()
            };
        }
    }
}
