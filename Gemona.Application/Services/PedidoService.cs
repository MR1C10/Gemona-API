using Gemona.Application.DTOs.Request.Pedido;
using Gemona.Application.DTOs.Response.Pedido;
using Gemona.Application.DTOs.Response.Cliente;
using Gemona.Application.DTOs.Response.Servico;
using Gemona.Application.DTOs.Response.SubCategoria;
using Gemona.Application.DTOs.Response.Estabelecimento;
using Gemona.Application.DTOs.Response.Avaliacao;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;
using Gemona.Domain.Enums;
using Gemona.Application.Exceptions;

namespace Gemona.Application.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IServicoRepository _servicoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IPedidoHistoricoRepository _pedidoHistoricoRepository;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            IServicoRepository servicoRepository,
            IClienteRepository clienteRepository,
            IPedidoHistoricoRepository pedidoHistoricoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _servicoRepository = servicoRepository;
            _clienteRepository = clienteRepository;
            _pedidoHistoricoRepository = pedidoHistoricoRepository;
        }

        public async Task<ApiResponse<IEnumerable<PedidoResponse>>> GetAllAsync()
        {
            var pedidos = await _pedidoRepository.GetAllActiveAsync();
            var response = pedidos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<PedidoResponse>>.SuccessResult(
                response, "Pedidos recuperados com sucesso");
        }

        public async Task<ApiResponse<PedidoResponse?>> GetByIdAsync(int id)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null)
            {
                throw new NotFoundException("Pedido", id);
            }

            var response = MapToResponse(pedido);
            return ApiResponse<PedidoResponse?>.SuccessResult(
                response, "Pedido encontrado com sucesso");
        }

        public async Task<ApiResponse<PedidoCompletoResponse?>> GetPedidoCompletoAsync(int pedidoId)
        {
            var pedido = await _pedidoRepository.GetPedidoCompletoAsync(pedidoId);
            if (pedido == null)
            {
                throw new NotFoundException("Pedido", pedidoId);
            }

            var response = MapToResponseCompleto(pedido);
            return ApiResponse<PedidoCompletoResponse?>.SuccessResult(
                response, "Pedido completo encontrado com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<PedidoResponse>>> GetPedidosByClienteAsync(int clienteId)
        {
            var pedidos = await _pedidoRepository.GetPedidosByClienteAsync(clienteId);
            var response = pedidos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<PedidoResponse>>.SuccessResult(
                response, "Pedidos do cliente recuperados com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<PedidoResponse>>> GetPedidosByEstabelecimentoAsync(int estabelecimentoId)
        {
            var pedidos = await _pedidoRepository.GetPedidosByEstabelecimentoAsync(estabelecimentoId);
            var response = pedidos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<PedidoResponse>>.SuccessResult(
                response, "Pedidos do estabelecimento recuperados com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<PedidoResponse>>> GetPedidosByStatusAsync(StatusPedido status)
        {
            var pedidos = await _pedidoRepository.GetPedidosByStatusAsync(status);
            var response = pedidos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<PedidoResponse>>.SuccessResult(
                response, $"Pedidos com status '{status}' recuperados com sucesso");
        }

        public async Task<ApiResponse<PagedResponse<PedidoResponse>>> GetPedidosPorPeriodoAsync(
            DateTime dataInicio, DateTime dataFim, int pageNumber = 1, int pageSize = 10)
        {
            if (dataInicio > dataFim)
            {
                throw new BusinessException("A data de início não pode ser maior que a data de fim");
            }

            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var pedidos = await _pedidoRepository.GetPedidosPorPeriodoAsync(dataInicio, dataFim);
            var totalRegistros = pedidos.Count();

            var pedidosPaginados = pedidos
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = pedidosPaginados.Select(MapToResponse).ToList();

            var pagedResponse = new PagedResponse<PedidoResponse>
            {
                Data = response,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalRegistros / (double)pageSize),
                TotalRecords = totalRegistros
            };

            return ApiResponse<PagedResponse<PedidoResponse>>.SuccessResult(
                pagedResponse, $"{totalRegistros} pedido(s) encontrado(s) no período");
        }

        public async Task<ApiResponse<decimal>> GetTotalVendasEstabelecimentoAsync(
            int estabelecimentoId, DateTime dataInicio, DateTime dataFim)
        {
            if (dataInicio > dataFim)
            {
                throw new BusinessException("A data de início não pode ser maior que a data de fim");
            }

            var total = await _pedidoRepository.GetTotalVendasEstabelecimentoAsync(
                estabelecimentoId, dataInicio, dataFim);
            
            return ApiResponse<decimal>.SuccessResult(
                total, $"Total de vendas: R$ {total:N2}");
        }

        public async Task<ApiResponse<PedidoResponse>> CreateAsync(CreatePedidoRequest request)
        {
            // Verificar se cliente existe
            var cliente = await _clienteRepository.GetByIdAsync(request.ClienteId);
            if (cliente == null)
            {
                throw new NotFoundException("Cliente", request.ClienteId);
            }

            // Verificar se serviço existe
            var servico = await _servicoRepository.GetByIdAsync(request.ServicoId);
            if (servico == null)
            {
                throw new NotFoundException("Serviço", request.ServicoId);
            }

            // Validar data de agendamento
            if (request.DataAgendamento.HasValue && request.DataAgendamento < DateTime.Now)
            {
                throw new BusinessException("A data de agendamento não pode ser no passado");
            }

            var pedido = new Pedido
            {
                ClienteId = request.ClienteId,
                ServicoId = request.ServicoId,
                DataSolicitacao = DateTime.UtcNow,
                DataAgendamento = request.DataAgendamento,
                Status = request.DataAgendamento.HasValue ? StatusPedido.Agendado : StatusPedido.Solicitado,
                ValorFinal = servico.Preco
            };

            var result = await _pedidoRepository.AddAsync(pedido);
            await _pedidoRepository.SaveChangesAsync();

            // Criar histórico inicial
            var historico = new PedidoHistorico
            {
                PedidoId = result.PedidoId,
                StatusAnterior = StatusPedido.Solicitado,
                StatusNovo = pedido.Status,
                DataAlteracao = DateTime.UtcNow
            };

            await _pedidoHistoricoRepository.AddAsync(historico);
            await _pedidoHistoricoRepository.SaveChangesAsync();

            var response = MapToResponse(result);
            return ApiResponse<PedidoResponse>.SuccessResult(
                response, "Pedido criado com sucesso");
        }

        public async Task<ApiResponse<PedidoResponse>> UpdateStatusAsync(int id, UpdateStatusPedidoRequest request)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null)
            {
                throw new NotFoundException("Pedido", id);
            }

            // Validações de transição de status
            var statusAnterior = pedido.Status;

            // Não permitir alterar pedido cancelado
            if (statusAnterior == StatusPedido.Cancelado)
            {
                throw new BusinessException("Não é possível alterar um pedido cancelado");
            }

            // Não permitir alterar pedido concluído (exceto para cancelado)
            if (statusAnterior == StatusPedido.Concluido && request.NovoStatus != StatusPedido.Cancelado)
            {
                throw new BusinessException("Não é possível alterar um pedido concluído");
            }

            // Validar data de agendamento
            if (request.DataAgendamento.HasValue && request.DataAgendamento < DateTime.Now)
            {
                throw new BusinessException("A data de agendamento não pode ser no passado");
            }

                // Atualizar pedido
                pedido.Status = request.NovoStatus;
                
                if (request.DataAgendamento.HasValue)
                    pedido.DataAgendamento = request.DataAgendamento;
                
                if (request.DataConclusao.HasValue)
                    pedido.DataConclusao = request.DataConclusao;
                
                if (request.ValorFinal.HasValue)
                    pedido.ValorFinal = request.ValorFinal;

                // Se está concluindo, definir data de conclusão automaticamente
                if (request.NovoStatus == StatusPedido.Concluido && !request.DataConclusao.HasValue)
                {
                    pedido.DataConclusao = DateTime.UtcNow;
                }

                pedido.DataAtualizacao = DateTime.UtcNow;

                await _pedidoRepository.UpdateAsync(pedido);
                await _pedidoRepository.SaveChangesAsync();

                // Criar histórico
                if (statusAnterior != request.NovoStatus)
                {
                    var historico = new PedidoHistorico
                    {
                        PedidoId = pedido.PedidoId,
                        StatusAnterior = statusAnterior,
                        StatusNovo = request.NovoStatus,
                        DataAlteracao = DateTime.UtcNow
                    };

                    await _pedidoHistoricoRepository.AddAsync(historico);
                    await _pedidoHistoricoRepository.SaveChangesAsync();
                }

                var response = MapToResponse(pedido);
                return ApiResponse<PedidoResponse>.SuccessResult(
                    response, "Status do pedido atualizado com sucesso");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var pedido = await _pedidoRepository.GetByIdAsync(id);
            if (pedido == null)
            {
                throw new NotFoundException("Pedido", id);
            }

            // Soft delete
            await _pedidoRepository.DeleteAsync(id);
            await _pedidoRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Pedido excluído com sucesso");
        }

        // Métodos de mapeamento
        private static PedidoResponse MapToResponse(Pedido pedido)
        {
            return new PedidoResponse
            {
                PedidoId = pedido.PedidoId,
                ClienteId = pedido.ClienteId,
                ClienteNome = pedido.Cliente?.Nome ?? string.Empty,
                ServicoId = pedido.ServicoId,
                ServicoNome = pedido.Servico?.Nome ?? string.Empty,
                EstabelecimentoNome = pedido.Servico?.Estabelecimento?.Nome ?? string.Empty,
                DataSolicitacao = pedido.DataSolicitacao,
                DataConclusao = pedido.DataConclusao,
                DataAgendamento = pedido.DataAgendamento,
                ValorFinal = pedido.ValorFinal,
                Status = pedido.Status,
                DataCriacao = pedido.DataCriacao,
                DataAtualizacao = pedido.DataAtualizacao,
                Ativo = pedido.Ativo
            };
        }

        private static PedidoCompletoResponse MapToResponseCompleto(Pedido pedido)
        {
            var response = new PedidoCompletoResponse
            {
                PedidoId = pedido.PedidoId,
                DataSolicitacao = pedido.DataSolicitacao,
                DataConclusao = pedido.DataConclusao,
                DataAgendamento = pedido.DataAgendamento,
                ValorFinal = pedido.ValorFinal,
                Status = pedido.Status,
                DataCriacao = pedido.DataCriacao,
                DataAtualizacao = pedido.DataAtualizacao,
                Ativo = pedido.Ativo
            };

            // Mapear Cliente
            if (pedido.Cliente != null)
            {
                response.Cliente = new ClienteResponse
                {
                    ClienteId = pedido.Cliente.Id,
                    Nome = pedido.Cliente.Nome,
                    Email = pedido.Cliente.Email ?? string.Empty,
                    Telefone = pedido.Cliente.PhoneNumber ?? string.Empty,
                    Cpf = pedido.Cliente.Cpf.Valor,
                    ImagemPerfilUrl = pedido.Cliente.ImagemPerfilUrl,
                    DataNascimento = pedido.Cliente.DataNascimento,
                    DataCriacao = pedido.Cliente.DataCriacao,
                    DataAtualizacao = pedido.Cliente.DataAtualizacao,
                    Ativo = pedido.Cliente.Ativo
                };
            }

            // Mapear Servico
            if (pedido.Servico != null)
            {
                response.Servico = new ServicoCompletoResponse
                {
                    ServicoId = pedido.Servico.ServicoId,
                    Nome = pedido.Servico.Nome,
                    Descricao = pedido.Servico.Descricao,
                    Preco = pedido.Servico.Preco,
                    ImagemServicoUrl = pedido.Servico.ImagemServicoUrl,
                    DataCriacao = pedido.Servico.DataCriacao,
                    DataAtualizacao = pedido.Servico.DataAtualizacao,
                    Ativo = pedido.Servico.Ativo
                };

                // Mapear SubCategoria do Serviço
                if (pedido.Servico.SubCategoria != null)
                {
                    response.Servico.SubCategoria = new SubCategoriaResponse
                    {
                        SubCategoriaId = pedido.Servico.SubCategoria.SubCategoriaId,
                        Nome = pedido.Servico.SubCategoria.Nome,
                        CategoriaId = pedido.Servico.SubCategoria.CategoriaId,
                        CategoriaNome = pedido.Servico.SubCategoria.Categoria?.Nome ?? string.Empty,
                        ImagemSubcategoriaUrl = pedido.Servico.SubCategoria.ImagemSubcategoriaUrl,
                        DataCriacao = pedido.Servico.SubCategoria.DataCriacao,
                        DataAtualizacao = pedido.Servico.SubCategoria.DataAtualizacao,
                        Ativo = pedido.Servico.SubCategoria.Ativo
                    };
                }

                // Mapear Estabelecimento do Serviço
                if (pedido.Servico.Estabelecimento != null)
                {
                    response.Servico.Estabelecimento = new EstabelecimentoResponse
                    {
                        EstabelecimentoId = pedido.Servico.Estabelecimento.EstabelecimentoId,
                        Nome = pedido.Servico.Estabelecimento.Nome,
                        Cnpj = pedido.Servico.Estabelecimento.Cnpj.Valor,
                        Telefone = pedido.Servico.Estabelecimento.Telefone,
                        Email = pedido.Servico.Estabelecimento.Email,
                        ImagemEstabelecimentoUrl = pedido.Servico.Estabelecimento.ImagemEstabelecimentoUrl,
                        DataCriacao = pedido.Servico.Estabelecimento.DataCriacao,
                        DataAtualizacao = pedido.Servico.Estabelecimento.DataAtualizacao,
                        Ativo = pedido.Servico.Estabelecimento.Ativo,
                        ProfissionalId = pedido.Servico.Estabelecimento.ProfissionalId
                    };
                }
            }

            // Mapear Histórico
            if (pedido.Historicos?.Any() == true)
            {
                response.Historico = pedido.Historicos.Select(h => new HistoricoResponse
                {
                    StatusAnterior = h.StatusAnterior,
                    StatusNovo = h.StatusNovo,
                    DataAlteracao = h.DataAlteracao
                }).ToList();
            }

            // Mapear Avaliação
            if (pedido.Avaliacao != null)
            {
                response.Avaliacao = new AvaliacaoResponse
                {
                    AvaliacaoId = pedido.Avaliacao.AvaliacaoId,
                    PedidoId = pedido.Avaliacao.PedidoId,
                    ClienteId = pedido.Avaliacao.ClienteId,
                    ClienteNome = pedido.Avaliacao.Cliente?.Nome ?? string.Empty,
                    Nota = pedido.Avaliacao.Nota,
                    Comentario = pedido.Avaliacao.Comentario,
                    Data = pedido.Avaliacao.Data,
                    ImagemComentarioUrl = pedido.Avaliacao.ImagemComentarioUrl,
                    DataCriacao = pedido.Avaliacao.DataCriacao,
                    DataAtualizacao = pedido.Avaliacao.DataAtualizacao,
                    Ativo = pedido.Avaliacao.Ativo
                };
            }

            return response;
        }
    }
}
