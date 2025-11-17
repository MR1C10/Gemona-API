using Gemona.Application.DTOs.Request.Avaliacao;
using Gemona.Application.DTOs.Response.Avaliacao;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;
using Gemona.Domain.Enums;
using Gemona.Application.Exceptions;

namespace Gemona.Application.Services
{
    public class AvaliacaoService : IAvaliacaoService
    {
        private readonly IAvaliacaoRepository _avaliacaoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IBlobStorageService _blobStorageService;

        public AvaliacaoService(
            IAvaliacaoRepository avaliacaoRepository,
            IPedidoRepository pedidoRepository,
            IClienteRepository clienteRepository,
            IBlobStorageService blobStorageService)
        {
            _avaliacaoRepository = avaliacaoRepository;
            _pedidoRepository = pedidoRepository;
            _clienteRepository = clienteRepository;
            _blobStorageService = blobStorageService;
        }

        public async Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAllAsync()
        {
            var avaliacoes = await _avaliacaoRepository.GetAllActiveAsync();
            var response = avaliacoes.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<AvaliacaoResponse>>.SuccessResult(
                response, "Avaliações recuperadas com sucesso");
        }

        public async Task<ApiResponse<AvaliacaoResponse?>> GetByIdAsync(int id)
        {
            var avaliacao = await _avaliacaoRepository.GetByIdAsync(id);
            if (avaliacao == null)
            {
                throw new NotFoundException("Avaliação", id);
            }

            var response = MapToResponse(avaliacao);
            return ApiResponse<AvaliacaoResponse?>.SuccessResult(
                response, "Avaliação encontrada com sucesso");
        }

        public async Task<ApiResponse<AvaliacaoResponse?>> GetAvaliacaoByPedidoAsync(int pedidoId)
        {
            var avaliacao = await _avaliacaoRepository.GetAvaliacaoByPedidoAsync(pedidoId);
            if (avaliacao == null)
            {
                throw new NotFoundException($"Avaliação para o pedido {pedidoId} não foi encontrada");
            }

            var response = MapToResponse(avaliacao);
            return ApiResponse<AvaliacaoResponse?>.SuccessResult(
                response, "Avaliação do pedido encontrada com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAvaliacoesByClienteAsync(int clienteId)
        {
            var avaliacoes = await _avaliacaoRepository.GetAvaliacoesByClienteAsync(clienteId);
            var response = avaliacoes.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<AvaliacaoResponse>>.SuccessResult(
                response, "Avaliações do cliente recuperadas com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAvaliacoesByEstabelecimentoAsync(int estabelecimentoId)
        {
            var avaliacoes = await _avaliacaoRepository.GetAvaliacoesByEstabelecimentoAsync(estabelecimentoId);
            var response = avaliacoes.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<AvaliacaoResponse>>.SuccessResult(
                response, "Avaliações do estabelecimento recuperadas com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAvaliacoesByNotaAsync(NotaAvaliacao nota)
        {
            var avaliacoes = await _avaliacaoRepository.GetAvaliacoesByNotaAsync(nota);
            var response = avaliacoes.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<AvaliacaoResponse>>.SuccessResult(
                response, $"Avaliações com nota {(int)nota} recuperadas com sucesso");
        }

        public async Task<ApiResponse<MediaAvaliacoesResponse>> GetMediaAvaliacoesEstabelecimentoAsync(int estabelecimentoId)
        {
            var avaliacoes = await _avaliacaoRepository.GetAvaliacoesByEstabelecimentoAsync(estabelecimentoId);
            
            if (!avaliacoes.Any())
            {
                throw new BusinessException("Este estabelecimento ainda não possui avaliações");
            }

            var estabelecimento = avaliacoes.First().Pedido?.Servico?.Estabelecimento;
            var media = await _avaliacaoRepository.GetMediaAvaliacoesEstabelecimentoAsync(estabelecimentoId);

            // Calcular distribuição de notas
            var distribuicao = avaliacoes
                .GroupBy(a => (int)a.Nota)
                .ToDictionary(g => g.Key, g => g.Count());

            // Pegar as 5 avaliações mais recentes
            var recentes = avaliacoes
                .OrderByDescending(a => a.Data)
                .Take(5)
                .Select(MapToResponse)
                .ToList();

            var response = new MediaAvaliacoesResponse
            {
                EstabelecimentoId = estabelecimentoId,
                EstabelecimentoNome = estabelecimento?.Nome ?? string.Empty,
                MediaGeral = media,
                TotalAvaliacoes = avaliacoes.Count(),
                DistribuicaoNotas = distribuicao,
                AvaliacoesRecentes = recentes
            };

            return ApiResponse<MediaAvaliacoesResponse>.SuccessResult(
                response, $"Média de avaliações: {media:N2}");
        }

        public async Task<ApiResponse<PagedResponse<AvaliacaoResponse>>> FiltrarAvaliacoesAsync(FiltrarAvaliacoesRequest request)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 10;

            if (request.DataInicio.HasValue && request.DataFim.HasValue && request.DataInicio > request.DataFim)
            {
                throw new BusinessException("A data de início não pode ser maior que a data de fim");
            }

                // Buscar todas as avaliações ativas
                var avaliacoesQuery = await _avaliacaoRepository.GetAllActiveAsync();

                // Aplicar filtros
                if (request.ClienteId.HasValue)
                {
                    avaliacoesQuery = avaliacoesQuery.Where(a => a.ClienteId == request.ClienteId.Value);
                }

                if (request.EstabelecimentoId.HasValue)
                {
                    avaliacoesQuery = avaliacoesQuery.Where(a => 
                        a.Pedido.Servico.EstabelecimentoId == request.EstabelecimentoId.Value);
                }

                if (request.Nota.HasValue)
                {
                    avaliacoesQuery = avaliacoesQuery.Where(a => a.Nota == request.Nota.Value);
                }

                if (request.DataInicio.HasValue)
                {
                    avaliacoesQuery = avaliacoesQuery.Where(a => a.Data >= request.DataInicio.Value);
                }

                if (request.DataFim.HasValue)
                {
                    avaliacoesQuery = avaliacoesQuery.Where(a => a.Data <= request.DataFim.Value);
                }

                // Ordenar por data (mais recentes primeiro)
                avaliacoesQuery = avaliacoesQuery.OrderByDescending(a => a.Data);

                var totalRegistros = avaliacoesQuery.Count();

                // Paginação
                var avaliacoes = avaliacoesQuery
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var response = avaliacoes.Select(MapToResponse).ToList();

                var pagedResponse = new PagedResponse<AvaliacaoResponse>
                {
                    Data = response,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = (int)Math.Ceiling(totalRegistros / (double)request.PageSize),
                    TotalRecords = totalRegistros
                };

                return ApiResponse<PagedResponse<AvaliacaoResponse>>.SuccessResult(
                    pagedResponse, $"{totalRegistros} avaliação(ões) encontrada(s)");
        }

        public async Task<ApiResponse<AvaliacaoResponse>> CreateAsync(CreateAvaliacaoRequest request)
        {
            // Verificar se cliente existe
            var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
            if (cliente == null)
            {
                throw new NotFoundException("Cliente", request.ClienteId);
            }

            // Verificar se pedido existe
            var pedido = await _pedidoRepository.GetByIdAsync(request.PedidoId);
            if (pedido == null)
            {
                throw new NotFoundException("Pedido", request.PedidoId);
            }

            // Verificar se o pedido pertence ao cliente
            if (pedido.ClienteId != request.ClienteId)
            {
                throw new BusinessException("Este pedido não pertence ao cliente informado");
            }

            // Verificar se o pedido foi concluído
            if (pedido.Status != StatusPedido.Concluido)
            {
                throw new BusinessException("Só é possível avaliar pedidos concluídos");
            }

            // Verificar se já existe avaliação para este pedido
            if (await _avaliacaoRepository.ClienteJaAvaliouPedidoAsync(request.ClienteId, request.PedidoId))
            {
                throw new BusinessException("Este pedido já foi avaliado");
            }

            // Validar nota
            if (!Enum.IsDefined(typeof(NotaAvaliacao), request.Nota))
            {
                throw new BusinessException("Nota inválida");
            }

            // Upload da imagem se fornecida
            string? imagemUrl = null;
            if (request.ImagemComentario != null)
            {
                var imageBytes = Convert.FromBase64String(request.ImagemComentario.Base64Data);
                using var imageStream = new MemoryStream(imageBytes);
                imagemUrl = await _blobStorageService.UploadImageAsync(
                    imageStream,
                    request.ImagemComentario.FileName,
                    request.ImagemComentario.ContentType
                );
            }

            var avaliacao = new Avaliacao
            {
                PedidoId = request.PedidoId,
                ClienteId = request.ClienteId,
                Nota = request.Nota,
                Comentario = request.Comentario,
                ImagemComentarioUrl = imagemUrl,
                Data = DateTime.UtcNow
            };

            var result = await _avaliacaoRepository.AddAsync(avaliacao);
            await _avaliacaoRepository.SaveChangesAsync();

            var response = MapToResponse(result);
            return ApiResponse<AvaliacaoResponse>.SuccessResult(
                response, "Avaliação criada com sucesso");
        }

        public async Task<ApiResponse<AvaliacaoResponse>> UpdateAsync(int id, UpdateAvaliacaoRequest request)
        {
            var avaliacao = await _avaliacaoRepository.GetByIdAsync(id);
            if (avaliacao == null)
            {
                throw new NotFoundException("Avaliação", id);
            }

            // Validar nota
            if (!Enum.IsDefined(typeof(NotaAvaliacao), request.Nota))
            {
                throw new BusinessException("Nota inválida");
            }

            // Upload da nova imagem se fornecida
            if (request.ImagemComentario != null)
            {
                // Deletar imagem antiga se existir
                if (!string.IsNullOrEmpty(avaliacao.ImagemComentarioUrl))
                {
                    await _blobStorageService.DeleteImageAsync(avaliacao.ImagemComentarioUrl);
                }

                // Upload da nova imagem
                var imageBytes = Convert.FromBase64String(request.ImagemComentario.Base64Data);
                using var imageStream = new MemoryStream(imageBytes);
                avaliacao.ImagemComentarioUrl = await _blobStorageService.UploadImageAsync(
                    imageStream,
                    request.ImagemComentario.FileName,
                    request.ImagemComentario.ContentType
                );
            }

            avaliacao.Nota = request.Nota;
            avaliacao.Comentario = request.Comentario;
            avaliacao.DataAtualizacao = DateTime.UtcNow;

            await _avaliacaoRepository.UpdateAsync(avaliacao);
            await _avaliacaoRepository.SaveChangesAsync();

            var response = MapToResponse(avaliacao);
            return ApiResponse<AvaliacaoResponse>.SuccessResult(
                response, "Avaliação atualizada com sucesso");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var avaliacao = await _avaliacaoRepository.GetByIdAsync(id);
            if (avaliacao == null)
            {
                throw new NotFoundException("Avaliação", id);
            }

            await _avaliacaoRepository.DeleteAsync(id);
            await _avaliacaoRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Avaliação excluída com sucesso");
        }

        public async Task<ApiResponse<bool>> ClienteJaAvaliouPedidoAsync(int clienteId, int pedidoId)
        {
            var jaAvaliou = await _avaliacaoRepository.ClienteJaAvaliouPedidoAsync(clienteId, pedidoId);
            return ApiResponse<bool>.SuccessResult(
                jaAvaliou, jaAvaliou ? "Cliente já avaliou este pedido" : "Cliente ainda não avaliou este pedido");
        }

        // Método de mapeamento
        private static AvaliacaoResponse MapToResponse(Avaliacao avaliacao)
        {
            return new AvaliacaoResponse
            {
                AvaliacaoId = avaliacao.AvaliacaoId,
                PedidoId = avaliacao.PedidoId,
                ClienteId = avaliacao.ClienteId,
                ClienteNome = avaliacao.Cliente?.Nome ?? string.Empty,
                ClienteImagemUrl = avaliacao.Cliente?.ImagemPerfilUrl,
                Nota = avaliacao.Nota,
                Comentario = avaliacao.Comentario,
                Data = avaliacao.Data,
                ImagemComentarioUrl = avaliacao.ImagemComentarioUrl,
                ServicoNome = avaliacao.Pedido?.Servico?.Nome ?? string.Empty,
                EstabelecimentoNome = avaliacao.Pedido?.Servico?.Estabelecimento?.Nome ?? string.Empty,
                DataCriacao = avaliacao.DataCriacao,
                DataAtualizacao = avaliacao.DataAtualizacao,
                Ativo = avaliacao.Ativo
            };
        }
    }
}
