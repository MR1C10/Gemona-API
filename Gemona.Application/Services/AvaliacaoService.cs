using Gemona.Application.DTOs.Request.Avaliacao;
using Gemona.Application.DTOs.Response.Avaliacao;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;
using Gemona.Domain.Enums;

namespace Gemona.Application.Services
{
    public class AvaliacaoService : IAvaliacaoService
    {
        private readonly IAvaliacaoRepository _avaliacaoRepository;
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IClienteRepository _clienteRepository;

        public AvaliacaoService(
            IAvaliacaoRepository avaliacaoRepository,
            IPedidoRepository pedidoRepository,
            IClienteRepository clienteRepository)
        {
            _avaliacaoRepository = avaliacaoRepository;
            _pedidoRepository = pedidoRepository;
            _clienteRepository = clienteRepository;
        }

        public async Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAllAsync()
        {
            try
            {
                var avaliacoes = await _avaliacaoRepository.GetAllActiveAsync();
                var response = avaliacoes.Select(MapToResponse);
                
                return ApiResponse<IEnumerable<AvaliacaoResponse>>.SuccessResult(
                    response, "Avaliações recuperadas com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AvaliacaoResponse>>.ErrorResult(
                    "Erro ao buscar avaliações", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AvaliacaoResponse?>> GetByIdAsync(int id)
        {
            try
            {
                var avaliacao = await _avaliacaoRepository.GetByIdAsync(id);
                if (avaliacao == null)
                {
                    return ApiResponse<AvaliacaoResponse?>.ErrorResult("Avaliação não encontrada");
                }

                var response = MapToResponse(avaliacao);
                return ApiResponse<AvaliacaoResponse?>.SuccessResult(
                    response, "Avaliação encontrada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<AvaliacaoResponse?>.ErrorResult(
                    "Erro ao buscar avaliação", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AvaliacaoResponse?>> GetAvaliacaoByPedidoAsync(int pedidoId)
        {
            try
            {
                var avaliacao = await _avaliacaoRepository.GetAvaliacaoByPedidoAsync(pedidoId);
                if (avaliacao == null)
                {
                    return ApiResponse<AvaliacaoResponse?>.ErrorResult("Avaliação não encontrada para este pedido");
                }

                var response = MapToResponse(avaliacao);
                return ApiResponse<AvaliacaoResponse?>.SuccessResult(
                    response, "Avaliação do pedido encontrada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<AvaliacaoResponse?>.ErrorResult(
                    "Erro ao buscar avaliação do pedido", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAvaliacoesByClienteAsync(int clienteId)
        {
            try
            {
                var avaliacoes = await _avaliacaoRepository.GetAvaliacoesByClienteAsync(clienteId);
                var response = avaliacoes.Select(MapToResponse);
                
                return ApiResponse<IEnumerable<AvaliacaoResponse>>.SuccessResult(
                    response, "Avaliações do cliente recuperadas com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AvaliacaoResponse>>.ErrorResult(
                    "Erro ao buscar avaliações do cliente", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAvaliacoesByEstabelecimentoAsync(int estabelecimentoId)
        {
            try
            {
                var avaliacoes = await _avaliacaoRepository.GetAvaliacoesByEstabelecimentoAsync(estabelecimentoId);
                var response = avaliacoes.Select(MapToResponse);
                
                return ApiResponse<IEnumerable<AvaliacaoResponse>>.SuccessResult(
                    response, "Avaliações do estabelecimento recuperadas com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AvaliacaoResponse>>.ErrorResult(
                    "Erro ao buscar avaliações do estabelecimento", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<IEnumerable<AvaliacaoResponse>>> GetAvaliacoesByNotaAsync(NotaAvaliacao nota)
        {
            try
            {
                var avaliacoes = await _avaliacaoRepository.GetAvaliacoesByNotaAsync(nota);
                var response = avaliacoes.Select(MapToResponse);
                
                return ApiResponse<IEnumerable<AvaliacaoResponse>>.SuccessResult(
                    response, $"Avaliações com nota {(int)nota} recuperadas com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<AvaliacaoResponse>>.ErrorResult(
                    "Erro ao buscar avaliações por nota", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<MediaAvaliacoesResponse>> GetMediaAvaliacoesEstabelecimentoAsync(int estabelecimentoId)
        {
            try
            {
                var avaliacoes = await _avaliacaoRepository.GetAvaliacoesByEstabelecimentoAsync(estabelecimentoId);
                
                if (!avaliacoes.Any())
                {
                    return ApiResponse<MediaAvaliacoesResponse>.ErrorResult(
                        "Este estabelecimento ainda não possui avaliações");
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
            catch (Exception ex)
            {
                return ApiResponse<MediaAvaliacoesResponse>.ErrorResult(
                    "Erro ao calcular média de avaliações", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PagedResponse<AvaliacaoResponse>>> FiltrarAvaliacoesAsync(FiltrarAvaliacoesRequest request)
        {
            try
            {
                if (request.PageNumber < 1) request.PageNumber = 1;
                if (request.PageSize < 1 || request.PageSize > 100) request.PageSize = 10;

                if (request.DataInicio.HasValue && request.DataFim.HasValue && request.DataInicio > request.DataFim)
                {
                    return ApiResponse<PagedResponse<AvaliacaoResponse>>.ErrorResult(
                        "A data de início não pode ser maior que a data de fim");
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
            catch (Exception ex)
            {
                return ApiResponse<PagedResponse<AvaliacaoResponse>>.ErrorResult(
                    "Erro ao filtrar avaliações", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AvaliacaoResponse>> CreateAsync(CreateAvaliacaoRequest request)
        {
            try
            {
                // Verificar se cliente existe
                var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
                if (cliente == null)
                {
                    return ApiResponse<AvaliacaoResponse>.ErrorResult("Cliente não encontrado");
                }

                // Verificar se pedido existe
                var pedido = await _pedidoRepository.GetByIdAsync(request.PedidoId);
                if (pedido == null)
                {
                    return ApiResponse<AvaliacaoResponse>.ErrorResult("Pedido não encontrado");
                }

                // Verificar se o pedido pertence ao cliente
                if (pedido.ClienteId != request.ClienteId)
                {
                    return ApiResponse<AvaliacaoResponse>.ErrorResult(
                        "Este pedido não pertence ao cliente informado");
                }

                // Verificar se o pedido foi concluído
                if (pedido.Status != StatusPedido.Concluido)
                {
                    return ApiResponse<AvaliacaoResponse>.ErrorResult(
                        "Só é possível avaliar pedidos concluídos");
                }

                // Verificar se já existe avaliação para este pedido
                if (await _avaliacaoRepository.ClienteJaAvaliouPedidoAsync(request.ClienteId, request.PedidoId))
                {
                    return ApiResponse<AvaliacaoResponse>.ErrorResult(
                        "Este pedido já foi avaliado");
                }

                // Validar nota
                if (!Enum.IsDefined(typeof(NotaAvaliacao), request.Nota))
                {
                    return ApiResponse<AvaliacaoResponse>.ErrorResult("Nota inválida");
                }

                var avaliacao = new Avaliacao
                {
                    PedidoId = request.PedidoId,
                    ClienteId = request.ClienteId,
                    Nota = request.Nota,
                    Comentario = request.Comentario,
                    ImagemComentarioUrl = request.ImagemComentarioUrl,
                    Data = DateTime.UtcNow
                };

                var result = await _avaliacaoRepository.AddAsync(avaliacao);
                await _avaliacaoRepository.SaveChangesAsync();

                var response = MapToResponse(result);
                return ApiResponse<AvaliacaoResponse>.SuccessResult(
                    response, "Avaliação criada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<AvaliacaoResponse>.ErrorResult(
                    "Erro ao criar avaliação", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AvaliacaoResponse>> UpdateAsync(int id, UpdateAvaliacaoRequest request)
        {
            try
            {
                var avaliacao = await _avaliacaoRepository.GetByIdAsync(id);
                if (avaliacao == null)
                {
                    return ApiResponse<AvaliacaoResponse>.ErrorResult("Avaliação não encontrada");
                }

                // Validar nota
                if (!Enum.IsDefined(typeof(NotaAvaliacao), request.Nota))
                {
                    return ApiResponse<AvaliacaoResponse>.ErrorResult("Nota inválida");
                }

                avaliacao.Nota = request.Nota;
                avaliacao.Comentario = request.Comentario;
                avaliacao.ImagemComentarioUrl = request.ImagemComentarioUrl;
                avaliacao.DataAtualizacao = DateTime.UtcNow;

                await _avaliacaoRepository.UpdateAsync(avaliacao);
                await _avaliacaoRepository.SaveChangesAsync();

                var response = MapToResponse(avaliacao);
                return ApiResponse<AvaliacaoResponse>.SuccessResult(
                    response, "Avaliação atualizada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<AvaliacaoResponse>.ErrorResult(
                    "Erro ao atualizar avaliação", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var avaliacao = await _avaliacaoRepository.GetByIdAsync(id);
                if (avaliacao == null)
                {
                    return ApiResponse<bool>.ErrorResult("Avaliação não encontrada");
                }

                await _avaliacaoRepository.DeleteAsync(id);
                await _avaliacaoRepository.SaveChangesAsync();

                return ApiResponse<bool>.SuccessResult(true, "Avaliação excluída com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResult(
                    "Erro ao excluir avaliação", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> ClienteJaAvaliouPedidoAsync(int clienteId, int pedidoId)
        {
            try
            {
                var jaAvaliou = await _avaliacaoRepository.ClienteJaAvaliouPedidoAsync(clienteId, pedidoId);
                return ApiResponse<bool>.SuccessResult(
                    jaAvaliou, jaAvaliou ? "Cliente já avaliou este pedido" : "Cliente ainda não avaliou este pedido");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResult(
                    "Erro ao verificar avaliação", new List<string> { ex.Message });
            }
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
