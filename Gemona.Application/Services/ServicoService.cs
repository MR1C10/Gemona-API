using Gemona.Application.DTOs.Request.Servico;
using Gemona.Application.DTOs.Response.Servico;
using Gemona.Application.DTOs.Response.SubCategoria;
using Gemona.Application.DTOs.Response.Estabelecimento;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;
using Gemona.Application.Exceptions;

namespace Gemona.Application.Services
{
    public class ServicoService : IServicoService
    {
        private readonly IServicoRepository _servicoRepository;
        private readonly IEstabelecimentoRepository _estabelecimentoRepository;
        private readonly ISubCategoriaRepository _subCategoriaRepository;

        public ServicoService(
            IServicoRepository servicoRepository,
            IEstabelecimentoRepository estabelecimentoRepository,
            ISubCategoriaRepository subCategoriaRepository)
        {
            _servicoRepository = servicoRepository;
            _estabelecimentoRepository = estabelecimentoRepository;
            _subCategoriaRepository = subCategoriaRepository;
        }

        public async Task<ApiResponse<IEnumerable<ServicoResponse>>> GetAllAsync()
        {
            var servicos = await _servicoRepository.GetAllActiveAsync();
            var response = servicos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<ServicoResponse>>.SuccessResult(
                response, "Serviços recuperados com sucesso");
        }

        public async Task<ApiResponse<ServicoResponse?>> GetByIdAsync(int id)
        {
            var servico = await _servicoRepository.GetByIdAsync(id);
            if (servico == null)
            {
                throw new NotFoundException("Serviço", id);
            }

            var response = MapToResponse(servico);
            return ApiResponse<ServicoResponse?>.SuccessResult(
                response, "Serviço encontrado com sucesso");
        }

        public async Task<ApiResponse<ServicoCompletoResponse?>> GetServicoCompletoAsync(int servicoId)
        {
            var servico = await _servicoRepository.GetServicoCompletoAsync(servicoId);
            if (servico == null)
            {
                throw new NotFoundException("Serviço", servicoId);
            }

            var response = MapToResponseCompleto(servico);
            return ApiResponse<ServicoCompletoResponse?>.SuccessResult(
                response, "Serviço completo encontrado com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<ServicoResponse>>> GetServicosByEstabelecimentoAsync(int estabelecimentoId)
        {
            var servicos = await _servicoRepository.GetServicosByEstabelecimentoAsync(estabelecimentoId);
            var response = servicos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<ServicoResponse>>.SuccessResult(
                response, "Serviços do estabelecimento recuperados com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<ServicoResponse>>> GetServicosByCategoriaAsync(int categoriaId)
        {
            var servicos = await _servicoRepository.GetServicosByCategoriaAsync(categoriaId);
            var response = servicos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<ServicoResponse>>.SuccessResult(
                response, "Serviços da categoria recuperados com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<ServicoResponse>>> GetServicosBySubCategoriaAsync(int subCategoriaId)
        {
            var servicos = await _servicoRepository.GetServicosBySubCategoriaAsync(subCategoriaId);
            var response = servicos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<ServicoResponse>>.SuccessResult(
                response, "Serviços da subcategoria recuperados com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<ServicoResponse>>> GetServicosByFaixaPrecoAsync(decimal precoMinimo, decimal precoMaximo)
        {
            if (precoMinimo < 0 || precoMaximo < 0)
            {
                throw new BusinessException("Os preços não podem ser negativos");
            }

            if (precoMinimo > precoMaximo)
            {
                throw new BusinessException("O preço mínimo não pode ser maior que o preço máximo");
            }

            var servicos = await _servicoRepository.GetServicosByFaixaPrecoAsync(precoMinimo, precoMaximo);
            var response = servicos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<ServicoResponse>>.SuccessResult(
                response, $"Serviços entre R$ {precoMinimo:N2} e R$ {precoMaximo:N2} recuperados com sucesso");
        }

        public async Task<ApiResponse<PagedResponse<ServicoResponse>>> BuscarServicosAsync(BuscarServicosRequest request)
        {
            // Validações
            if (request.PageNumber < 1)
                request.PageNumber = 1;
            
            if (request.PageSize < 1 || request.PageSize > 100)
                request.PageSize = 10;

            if (request.PrecoMinimo.HasValue && request.PrecoMinimo < 0)
            {
                throw new BusinessException("O preço mínimo não pode ser negativo");
            }

            if (request.PrecoMaximo.HasValue && request.PrecoMaximo < 0)
            {
                throw new BusinessException("O preço máximo não pode ser negativo");
            }

            if (request.PrecoMinimo.HasValue && request.PrecoMaximo.HasValue && 
                request.PrecoMinimo > request.PrecoMaximo)
            {
                throw new BusinessException("O preço mínimo não pode ser maior que o preço máximo");
            }

                // Buscar todos os serviços ativos
                var servicosQuery = await _servicoRepository.GetAllActiveAsync();

                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(request.Termo))
                {
                    var termo = request.Termo.ToLower();
                    servicosQuery = servicosQuery.Where(s => 
                        s.Nome.ToLower().Contains(termo) || 
                        (s.Descricao != null && s.Descricao.ToLower().Contains(termo)));
                }

                if (request.SubCategoriaId.HasValue)
                {
                    servicosQuery = servicosQuery.Where(s => s.SubCategoriaId == request.SubCategoriaId.Value);
                }
                else if (request.CategoriaId.HasValue)
                {
                    servicosQuery = servicosQuery.Where(s => s.SubCategoria.CategoriaId == request.CategoriaId.Value);
                }

                if (request.PrecoMinimo.HasValue)
                {
                    servicosQuery = servicosQuery.Where(s => s.Preco >= request.PrecoMinimo.Value);
                }

                if (request.PrecoMaximo.HasValue)
                {
                    servicosQuery = servicosQuery.Where(s => s.Preco <= request.PrecoMaximo.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.Cidade))
                {
                    servicosQuery = servicosQuery.Where(s => 
                        s.Estabelecimento.Endereco.Cidade.ToLower() == request.Cidade.ToLower());
                }

                // Filtro por proximidade (se fornecido lat/long e raio)
                if (request.Latitude.HasValue && request.Longitude.HasValue && request.RaioKm.HasValue)
                {
                    servicosQuery = servicosQuery.Where(s => 
                        CalcularDistancia(
                            request.Latitude.Value, 
                            request.Longitude.Value, 
                            s.Estabelecimento.Endereco.Latitude, 
                            s.Estabelecimento.Endereco.Longitude) <= request.RaioKm.Value);
                }

                // Total de registros
                var totalRegistros = servicosQuery.Count();

                // Paginação
                var servicos = servicosQuery
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var servicosResponse = servicos.Select(MapToResponse).ToList();

                var pagedResponse = new PagedResponse<ServicoResponse>
                {
                    Data = servicosResponse,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling(totalRegistros / (double)request.PageSize),
                    TotalRecords = totalRegistros
                };

                return ApiResponse<PagedResponse<ServicoResponse>>.SuccessResult(
                    pagedResponse, $"{totalRegistros} serviço(s) encontrado(s)");
        }

        public async Task<ApiResponse<ServicoResponse>> CreateAsync(CreateServicoRequest request)
        {
            // Validações
            if (request.Preco < 0)
            {
                throw new BusinessException("O preço não pode ser negativo");
            }

            // Verificar se estabelecimento existe
            var estabelecimento = await _estabelecimentoRepository.GetByIdAsync(request.EstabelecimentoId);
            if (estabelecimento == null)
            {
                throw new NotFoundException("Estabelecimento", request.EstabelecimentoId);
            }

            // Verificar se subcategoria existe
            var subCategoria = await _subCategoriaRepository.GetByIdAsync(request.SubCategoriaId);
            if (subCategoria == null)
            {
                throw new NotFoundException("Subcategoria", request.SubCategoriaId);
            }

            var servico = new Servico
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                SubCategoriaId = request.SubCategoriaId,
                Preco = request.Preco,
                ImagemServicoUrl = request.ImagemServicoUrl,
                EstabelecimentoId = request.EstabelecimentoId
            };

            var result = await _servicoRepository.AddAsync(servico);
            await _servicoRepository.SaveChangesAsync();

            var response = MapToResponse(result);
            return ApiResponse<ServicoResponse>.SuccessResult(
                response, "Serviço criado com sucesso");
        }

        public async Task<ApiResponse<ServicoResponse>> UpdateAsync(int id, UpdateServicoRequest request)
        {
            var servico = await _servicoRepository.GetByIdAsync(id);
            if (servico == null)
            {
                throw new NotFoundException("Serviço", id);
            }

            // Validações
            if (request.Preco < 0)
            {
                throw new BusinessException("O preço não pode ser negativo");
            }

            // Verificar se subcategoria existe
            var subCategoria = await _subCategoriaRepository.GetByIdAsync(request.SubCategoriaId);
            if (subCategoria == null)
            {
                throw new NotFoundException("Subcategoria", request.SubCategoriaId);
            }

            servico.Nome = request.Nome;
            servico.Descricao = request.Descricao;
            servico.SubCategoriaId = request.SubCategoriaId;
            servico.Preco = request.Preco;
            servico.ImagemServicoUrl = request.ImagemServicoUrl;
            servico.DataAtualizacao = DateTime.UtcNow;

            await _servicoRepository.UpdateAsync(servico);
            await _servicoRepository.SaveChangesAsync();

            var response = MapToResponse(servico);
            return ApiResponse<ServicoResponse>.SuccessResult(
                response, "Serviço atualizado com sucesso");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var servico = await _servicoRepository.GetByIdAsync(id);
            if (servico == null)
            {
                throw new NotFoundException("Serviço", id);
            }

            await _servicoRepository.DeleteAsync(id);
            await _servicoRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Serviço excluído com sucesso");
        }

        // Métodos auxiliares
        private static double CalcularDistancia(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            // Fórmula de Haversine para calcular distância entre dois pontos (em km)
            const double R = 6371; // Raio da Terra em km

            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        // Métodos de mapeamento
        private static ServicoResponse MapToResponse(Servico servico)
        {
            return new ServicoResponse
            {
                ServicoId = servico.ServicoId,
                Nome = servico.Nome,
                Descricao = servico.Descricao,
                SubCategoriaId = servico.SubCategoriaId,
                SubCategoriaNome = servico.SubCategoria?.Nome ?? string.Empty,
                CategoriaNome = servico.SubCategoria?.Categoria?.Nome ?? string.Empty,
                Preco = servico.Preco,
                ImagemServicoUrl = servico.ImagemServicoUrl,
                EstabelecimentoId = servico.EstabelecimentoId,
                EstabelecimentoNome = servico.Estabelecimento?.Nome ?? string.Empty,
                DataCriacao = servico.DataCriacao,
                DataAtualizacao = servico.DataAtualizacao,
                Ativo = servico.Ativo
            };
        }

        private static ServicoCompletoResponse MapToResponseCompleto(Servico servico)
        {
            var response = new ServicoCompletoResponse
            {
                ServicoId = servico.ServicoId,
                Nome = servico.Nome,
                Descricao = servico.Descricao,
                Preco = servico.Preco,
                ImagemServicoUrl = servico.ImagemServicoUrl,
                DataCriacao = servico.DataCriacao,
                DataAtualizacao = servico.DataAtualizacao,
                Ativo = servico.Ativo
            };

            // Mapear SubCategoria
            if (servico.SubCategoria != null)
            {
                response.SubCategoria = new SubCategoriaResponse
                {
                    SubCategoriaId = servico.SubCategoria.SubCategoriaId,
                    Nome = servico.SubCategoria.Nome,
                    CategoriaId = servico.SubCategoria.CategoriaId,
                    CategoriaNome = servico.SubCategoria.Categoria?.Nome ?? string.Empty,
                    ImagemSubcategoriaUrl = servico.SubCategoria.ImagemSubcategoriaUrl,
                    DataCriacao = servico.SubCategoria.DataCriacao,
                    DataAtualizacao = servico.SubCategoria.DataAtualizacao,
                    Ativo = servico.SubCategoria.Ativo
                };
            }

            // Mapear Estabelecimento
            if (servico.Estabelecimento != null)
            {
                response.Estabelecimento = new EstabelecimentoResponse
                {
                    EstabelecimentoId = servico.Estabelecimento.EstabelecimentoId,
                    Nome = servico.Estabelecimento.Nome,
                    Cnpj = servico.Estabelecimento.Cnpj.Valor,
                    Telefone = servico.Estabelecimento.Telefone,
                    Email = servico.Estabelecimento.Email,
                    ImagemEstabelecimentoUrl = servico.Estabelecimento.ImagemEstabelecimentoUrl,
                    DataCriacao = servico.Estabelecimento.DataCriacao,
                    DataAtualizacao = servico.Estabelecimento.DataAtualizacao,
                    Ativo = servico.Estabelecimento.Ativo,
                    ProfissionalId = servico.Estabelecimento.ProfissionalId
                };
            }

            return response;
        }
    }
}
