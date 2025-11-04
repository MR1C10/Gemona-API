using Gemona.Application.DTOs.Request.SubCategoria;
using Gemona.Application.DTOs.Response.SubCategoria;
using Gemona.Application.DTOs.Response.Servico;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Exceptions;
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
            var subCategorias = await _subCategoriaRepository.GetAllActiveAsync();
            var response = subCategorias.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<SubCategoriaResponse>>.SuccessResult(
                response, "Subcategorias recuperadas com sucesso");
        }

        public async Task<ApiResponse<SubCategoriaResponse?>> GetByIdAsync(int id)
        {
            var subCategoria = await _subCategoriaRepository.GetByIdAsync(id);
            if (subCategoria == null)
            {
                throw new NotFoundException("Subcategoria", id);
            }

            var response = MapToResponse(subCategoria);
            return ApiResponse<SubCategoriaResponse?>.SuccessResult(
                response, "Subcategoria encontrada com sucesso");
        }

        public async Task<ApiResponse<SubCategoriaWithServicosResponse?>> GetSubCategoriaWithServicosAsync(int subCategoriaId)
        {
            var subCategoria = await _subCategoriaRepository.GetSubCategoriaWithServicosAsync(subCategoriaId);
            if (subCategoria == null)
            {
                throw new NotFoundException("Subcategoria", subCategoriaId);
            }

            var response = MapToResponseWithServicos(subCategoria);
            return ApiResponse<SubCategoriaWithServicosResponse?>.SuccessResult(
                response, "Subcategoria com serviços encontrada com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<SubCategoriaResponse>>> GetSubCategoriasByCategoriaAsync(int categoriaId)
        {
            var subCategorias = await _subCategoriaRepository.GetSubCategoriasByCategoriaAsync(categoriaId);
            var response = subCategorias.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<SubCategoriaResponse>>.SuccessResult(
                response, "Subcategorias da categoria encontradas com sucesso");
        }

        public async Task<ApiResponse<SubCategoriaResponse>> CreateAsync(CreateSubCategoriaRequest request)
        {
            // Verificar se categoria existe
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId);
            if (categoria == null)
            {
                throw new NotFoundException("Categoria", request.CategoriaId);
            }

            // Validar se nome já existe na categoria
            if (await _subCategoriaRepository.NomeExistsAsync(request.Nome, request.CategoriaId))
            {
                throw new BusinessException("Já existe uma subcategoria com este nome nesta categoria");
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

        public async Task<ApiResponse<SubCategoriaResponse>> UpdateAsync(int id, UpdateSubCategoriaRequest request)
        {
            var subCategoria = await _subCategoriaRepository.GetByIdAsync(id);
            if (subCategoria == null)
            {
                throw new NotFoundException("Subcategoria", id);
            }

            // Verificar se nova categoria existe
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId);
            if (categoria == null)
            {
                throw new NotFoundException("Categoria", request.CategoriaId);
            }

            // Verificar se novo nome já existe na categoria (exceto na própria subcategoria)
            var subCategoriaExistente = await _subCategoriaRepository.GetByNomeAsync(request.Nome);
            if (subCategoriaExistente != null && 
                subCategoriaExistente.SubCategoriaId != id && 
                subCategoriaExistente.CategoriaId == request.CategoriaId)
            {
                throw new BusinessException("Já existe uma subcategoria com este nome nesta categoria");
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

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var subCategoria = await _subCategoriaRepository.GetByIdAsync(id);
            if (subCategoria == null)
            {
                throw new NotFoundException("Subcategoria", id);
            }

            await _subCategoriaRepository.DeleteAsync(id);
            await _subCategoriaRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Subcategoria excluída com sucesso");
        }

        public async Task<ApiResponse<bool>> NomeExistsAsync(string nome, int categoriaId)
        {
            var exists = await _subCategoriaRepository.NomeExistsAsync(nome, categoriaId);
            return ApiResponse<bool>.SuccessResult(exists, "Verificação realizada com sucesso");
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
