using Gemona.Application.DTOs.Request.Estabelecimento;
using Gemona.Application.DTOs.Response.Estabelecimento;
using Gemona.Application.DTOs.Response.Servico;
using Gemona.Application.DTOs.Response.Endereco;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;
using Gemona.Application.Exceptions;

namespace Gemona.Application.Services
{
    public class EstabelecimentoService : IEstabelecimentoService
    {
        private readonly IEstabelecimentoRepository _estabelecimentoRepository;
        private readonly IProfissionalRepository _profissionalRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IGeocodingService _geocodingService;

        public EstabelecimentoService(
            IEstabelecimentoRepository estabelecimentoRepository,
            IProfissionalRepository profissionalRepository,
            IEnderecoRepository enderecoRepository,
            IBlobStorageService blobStorageService,
            IGeocodingService geocodingService)
        {
            _estabelecimentoRepository = estabelecimentoRepository;
            _profissionalRepository = profissionalRepository;
            _enderecoRepository = enderecoRepository;
            _blobStorageService = blobStorageService;
            _geocodingService = geocodingService;
        }

        public async Task<ApiResponse<IEnumerable<EstabelecimentoResponse>>> GetAllAsync()
        {
            var estabelecimentos = await _estabelecimentoRepository.GetAllActiveAsync();
            var response = estabelecimentos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<EstabelecimentoResponse>>.SuccessResult(
                response, "Estabelecimentos recuperados com sucesso");
        }

        public async Task<ApiResponse<EstabelecimentoResponse?>> GetByIdAsync(int id)
        {
            var estabelecimento = await _estabelecimentoRepository.GetByIdAsync(id);
            if (estabelecimento == null)
            {
                throw new NotFoundException("Estabelecimento", id);
            }

            var response = MapToResponse(estabelecimento);
            return ApiResponse<EstabelecimentoResponse?>.SuccessResult(
                response, "Estabelecimento encontrado com sucesso");
        }

        public async Task<ApiResponse<EstabelecimentoResponse?>> GetByCnpjAsync(string cnpj)
        {
            var cnpjValueObject = new Cnpj(cnpj);
            var estabelecimento = await _estabelecimentoRepository.GetByCnpjAsync(cnpjValueObject);
            if (estabelecimento == null)
            {
                throw new NotFoundException($"Estabelecimento com CNPJ '{cnpj}' não foi encontrado");
            }

            var response = MapToResponse(estabelecimento);
            return ApiResponse<EstabelecimentoResponse?>.SuccessResult(
                response, "Estabelecimento encontrado com sucesso");
        }

        public async Task<ApiResponse<EstabelecimentoCompletoResponse?>> GetEstabelecimentoCompletoAsync(int id)
        {
            var estabelecimento = await _estabelecimentoRepository.GetEstabelecimentoCompletoAsync(id);
            if (estabelecimento == null)
            {
                throw new NotFoundException("Estabelecimento", id);
            }

            var response = MapToResponseCompleto(estabelecimento);
            return ApiResponse<EstabelecimentoCompletoResponse?>.SuccessResult(
                response, "Estabelecimento completo encontrado com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<EstabelecimentoResponse>>> GetEstabelecimentosByProfissionalAsync(int profissionalId)
        {
            var estabelecimentos = await _estabelecimentoRepository.GetEstabelecimentosByProfissionalAsync(profissionalId);
            var response = estabelecimentos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<EstabelecimentoResponse>>.SuccessResult(
                response, "Estabelecimentos do profissional encontrados com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<EstabelecimentoResponse>>> GetEstabelecimentosByCidadeAsync(string cidade)
        {
            var estabelecimentos = await _estabelecimentoRepository.GetEstabelecimentosByCidadeAsync(cidade);
            var response = estabelecimentos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<EstabelecimentoResponse>>.SuccessResult(
                response, $"Estabelecimentos da cidade {cidade} encontrados com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<EstabelecimentoResponse>>> GetEstabelecimentosProximosAsync(decimal latitude, decimal longitude, double raioKm)
        {
            var estabelecimentos = await _estabelecimentoRepository.GetEstabelecimentosProximosAsync(latitude, longitude, raioKm);
            var response = estabelecimentos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<EstabelecimentoResponse>>.SuccessResult(
                response, $"Estabelecimentos próximos encontrados com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<EstabelecimentoResponse>>> BuscarEstabelecimentosAsync(string termo)
        {
            var estabelecimentos = await _estabelecimentoRepository.BuscarEstabelecimentosAsync(termo);
            var response = estabelecimentos.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<EstabelecimentoResponse>>.SuccessResult(
                response, $"Estabelecimentos encontrados para '{termo}'");
        }

        public async Task<ApiResponse<EstabelecimentoResponse>> CreateAsync(CreateEstabelecimentoRequest request)
        {
            // Validações
            var cnpjValueObject = new Cnpj(request.Cnpj);
            
            var cnpjExists = await _estabelecimentoRepository.CnpjExistsAsync(cnpjValueObject);
            if (cnpjExists)
            {
                throw new BusinessException("Já existe um estabelecimento com este CNPJ");
            }

            // Verificar se profissional existe
            var profissional = await _profissionalRepository.GetByIdAsync(request.ProfissionalId);
            if (profissional == null)
            {
                throw new NotFoundException("Profissional", request.ProfissionalId);
            }

            // Buscar coordenadas automaticamente se não fornecidas
            decimal latitude = request.Latitude;
            decimal longitude = request.Longitude;

            if (latitude == 0 && longitude == 0)
            {
                var enderecoCompleto = $"{request.Rua}, {request.Numero}, {request.Bairro}, {request.Cidade}, {request.Estado}, {request.Cep}, Brasil";
                var coordenadas = await _geocodingService.BuscarCoordenadasAsync(enderecoCompleto);
                
                if (coordenadas.HasValue)
                {
                    latitude = coordenadas.Value.Latitude ?? 0;
                    longitude = coordenadas.Value.Longitude ?? 0;
                }
            }

            // Criar endereço
            var endereco = new Endereco
            {
                Rua = request.Rua,
                Numero = request.Numero,
                Bairro = request.Bairro,
                Complemento = request.Complemento,
                Cidade = request.Cidade,
                Estado = request.Estado,
                Cep = new Cep(request.Cep),
                Latitude = latitude,
                Longitude = longitude
            };

            var enderecoResult = await _enderecoRepository.AddAsync(endereco);
            await _enderecoRepository.SaveChangesAsync();

            // Upload da imagem se fornecida
            string? imagemUrl = null;
            if (request.ImagemEstabelecimento != null)
            {
                var imageBytes = Convert.FromBase64String(request.ImagemEstabelecimento.Base64Data);
                using var imageStream = new MemoryStream(imageBytes);
                imagemUrl = await _blobStorageService.UploadImageAsync(
                    imageStream,
                    request.ImagemEstabelecimento.FileName,
                    request.ImagemEstabelecimento.ContentType
                );
            }

            // Criar estabelecimento
            var estabelecimento = new Estabelecimento
            {
                Nome = request.Nome,
                Cnpj = cnpjValueObject,
                Telefone = request.Telefone,
                Email = request.Email,
                Descricao = request.Descricao,
                ImagemEstabelecimentoUrl = imagemUrl,
                ProfissionalId = request.ProfissionalId,
                EnderecoId = enderecoResult.EnderecoId,
                HorariosFuncionamento = request.Horarios.Select(h => new HorarioFuncionamento
                {
                    DiaSemana = (Domain.Enums.DiaSemana)h.DiaSemana,
                    HoraAbertura = h.HoraAbertura,
                    HoraFechamento = h.HoraFechamento,
                    Fechado = h.Fechado
                }).ToList()
            };

            var result = await _estabelecimentoRepository.AddAsync(estabelecimento);
            await _estabelecimentoRepository.SaveChangesAsync();

            var response = new EstabelecimentoResponse
            {
                EstabelecimentoId = result.EstabelecimentoId,
                Nome = result.Nome,
                Cnpj = result.Cnpj.Valor,
                Telefone = result.Telefone,
                Email = result.Email,
                Descricao = result.Descricao,
                ImagemEstabelecimentoUrl = result.ImagemEstabelecimentoUrl,
                ProfissionalId = result.ProfissionalId,
                ProfissionalNome = profissional.Nome,
                Ativo = result.Ativo,
                DataCriacao = result.DataCriacao,
                DataAtualizacao = result.DataAtualizacao,
                Endereco = new EnderecoResponse
                {
                    EnderecoId = endereco.EnderecoId,
                    Rua = endereco.Rua,
                    Numero = endereco.Numero,
                    Bairro = endereco.Bairro,
                    Complemento = endereco.Complemento,
                    Cidade = endereco.Cidade,
                    Estado = endereco.Estado,
                    Cep = endereco.Cep.Valor,
                    Latitude = endereco.Latitude,
                    Longitude = endereco.Longitude,
                    DataCriacao = endereco.DataCriacao,
                    DataAtualizacao = endereco.DataAtualizacao,
                    Ativo = endereco.Ativo
                },
                Horarios = result.HorariosFuncionamento.Select(h => new HorarioFuncionamentoResponse
                {
                    DiaSemana = h.DiaSemana,
                    HoraAbertura = h.HoraAbertura,
                    HoraFechamento = h.HoraFechamento,
                    Fechado = h.Fechado
                }).ToList()
            };

            return ApiResponse<EstabelecimentoResponse>.SuccessResult(
                response, "Estabelecimento criado com sucesso");
        }

        public async Task<ApiResponse<EstabelecimentoResponse>> UpdateAsync(int id, UpdateEstabelecimentoRequest request)
        {
            var estabelecimento = await _estabelecimentoRepository.GetByIdAsync(id);
            if (estabelecimento == null)
            {
                throw new NotFoundException("Estabelecimento", id);
            }

            // Upload da nova imagem se fornecida
            if (request.ImagemEstabelecimento != null)
            {
                // Deletar imagem antiga se existir
                if (!string.IsNullOrEmpty(estabelecimento.ImagemEstabelecimentoUrl))
                {
                    await _blobStorageService.DeleteImageAsync(estabelecimento.ImagemEstabelecimentoUrl);
                }

                // Upload da nova imagem
                var imageBytes = Convert.FromBase64String(request.ImagemEstabelecimento.Base64Data);
                using var imageStream = new MemoryStream(imageBytes);
                estabelecimento.ImagemEstabelecimentoUrl = await _blobStorageService.UploadImageAsync(
                    imageStream,
                    request.ImagemEstabelecimento.FileName,
                    request.ImagemEstabelecimento.ContentType
                );
            }

            // Atualizar estabelecimento
            estabelecimento.Nome = request.Nome;
            estabelecimento.Telefone = request.Telefone;
            estabelecimento.Email = request.Email;
            estabelecimento.Descricao = request.Descricao;
            estabelecimento.DataAtualizacao = DateTime.UtcNow;

            // Atualizar endereço
            var endereco = await _enderecoRepository.GetByIdAsync(estabelecimento.EnderecoId);
            if (endereco != null)
            {
                endereco.Rua = request.Rua;
                endereco.Numero = request.Numero;
                endereco.Bairro = request.Bairro;
                endereco.Complemento = request.Complemento;
                endereco.Cidade = request.Cidade;
                endereco.Estado = request.Estado;
                endereco.Cep = new Cep(request.Cep);
                endereco.Latitude = request.Latitude;
                endereco.Longitude = request.Longitude;
                endereco.DataAtualizacao = DateTime.UtcNow;

                await _enderecoRepository.UpdateAsync(endereco);
                await _enderecoRepository.SaveChangesAsync();
            }

            await _estabelecimentoRepository.UpdateAsync(estabelecimento);
            await _estabelecimentoRepository.SaveChangesAsync();

            var response = MapToResponse(estabelecimento);
            return ApiResponse<EstabelecimentoResponse>.SuccessResult(
                response, "Estabelecimento atualizado com sucesso");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var estabelecimento = await _estabelecimentoRepository.GetByIdAsync(id);
            if (estabelecimento == null)
            {
                throw new NotFoundException("Estabelecimento", id);
            }

            await _estabelecimentoRepository.DeleteAsync(id);
            await _estabelecimentoRepository.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResult(true, "Estabelecimento excluído com sucesso");
        }

        public async Task<ApiResponse<bool>> CnpjExistsAsync(string cnpj)
        {
            var cnpjValueObject = new Cnpj(cnpj);
            var exists = await _estabelecimentoRepository.CnpjExistsAsync(cnpjValueObject);
            return ApiResponse<bool>.SuccessResult(exists, "Verificação realizada com sucesso");
        }

        // Métodos de mapeamento
        private static EstabelecimentoResponse MapToResponse(Estabelecimento estabelecimento)
        {
            return new EstabelecimentoResponse
            {
                EstabelecimentoId = estabelecimento.EstabelecimentoId,
                Nome = estabelecimento.Nome,
                Cnpj = estabelecimento.Cnpj.Valor,
                Telefone = estabelecimento.Telefone,
                Email = estabelecimento.Email,
                ImagemEstabelecimentoUrl = estabelecimento.ImagemEstabelecimentoUrl,
                DataCriacao = estabelecimento.DataCriacao,
                DataAtualizacao = estabelecimento.DataAtualizacao,
                Ativo = estabelecimento.Ativo,
                ProfissionalId = estabelecimento.ProfissionalId
            };
        }

        private static EstabelecimentoCompletoResponse MapToResponseCompleto(Estabelecimento estabelecimento)
        {
            var response = new EstabelecimentoCompletoResponse
            {
                EstabelecimentoId = estabelecimento.EstabelecimentoId,
                Nome = estabelecimento.Nome,
                Cnpj = estabelecimento.Cnpj.Valor,
                Telefone = estabelecimento.Telefone,
                Email = estabelecimento.Email,
                Descricao = estabelecimento.Descricao,
                ImagemEstabelecimentoUrl = estabelecimento.ImagemEstabelecimentoUrl,
                DataCriacao = estabelecimento.DataCriacao,
                DataAtualizacao = estabelecimento.DataAtualizacao,
                Ativo = estabelecimento.Ativo
            };

            // Mapear Profissional
            if (estabelecimento.Profissional != null)
            {
                response.Profissional = new DTOs.Response.Profissional.ProfissionalResponse
                {
                    ProfissionalId = estabelecimento.Profissional.Id,
                    Nome = estabelecimento.Profissional.Nome,
                    Email = estabelecimento.Profissional.Email ?? string.Empty,
                    Telefone = estabelecimento.Profissional.PhoneNumber ?? string.Empty,
                    Cpf = estabelecimento.Profissional.Cpf.Valor,
                    ImagemPerfilUrl = estabelecimento.Profissional.ImagemPerfilUrl,
                    DataNascimento = estabelecimento.Profissional.DataNascimento,
                    DataCriacao = estabelecimento.Profissional.DataCriacao,
                    DataAtualizacao = estabelecimento.Profissional.DataAtualizacao,
                    Ativo = estabelecimento.Profissional.Ativo
                };
            }

            // Mapear Endereco
            if (estabelecimento.Endereco != null)
            {
                response.Endereco = new EnderecoResponse
                {
                    EnderecoId = estabelecimento.Endereco.EnderecoId,
                    Rua = estabelecimento.Endereco.Rua,
                    Numero = estabelecimento.Endereco.Numero,
                    Bairro = estabelecimento.Endereco.Bairro,
                    Complemento = estabelecimento.Endereco.Complemento,
                    Cidade = estabelecimento.Endereco.Cidade,
                    Estado = estabelecimento.Endereco.Estado,
                    Cep = estabelecimento.Endereco.Cep.Valor,
                    Latitude = estabelecimento.Endereco.Latitude,
                    Longitude = estabelecimento.Endereco.Longitude,
                    DataCriacao = estabelecimento.Endereco.DataCriacao,
                    DataAtualizacao = estabelecimento.Endereco.DataAtualizacao,
                    Ativo = estabelecimento.Endereco.Ativo
                };
            }

            // Mapear Servicos
            if (estabelecimento.Servicos?.Any() == true)
            {
                response.Servicos = estabelecimento.Servicos.Select(s => new ServicoResponse
                {
                    ServicoId = s.ServicoId,
                    Nome = s.Nome,
                    Descricao = s.Descricao,
                    Preco = s.Preco,
                    ImagemServicoUrl = s.ImagemServicoUrl,
                    EstabelecimentoId = s.EstabelecimentoId,
                    SubCategoriaId = s.SubCategoriaId,
                    SubCategoriaNome = s.SubCategoria?.Nome ?? string.Empty,
                    DataCriacao = s.DataCriacao,
                    DataAtualizacao = s.DataAtualizacao,
                    Ativo = s.Ativo
                }).ToList();
            }

            // Mapear Horarios (HorariosFuncionamento)
            if (estabelecimento.HorariosFuncionamento?.Any() == true)
            {
                response.Horarios = estabelecimento.HorariosFuncionamento.Select(h => new DTOs.Shared.HorarioFuncionamentoResponse
                {
                    DiaSemana = h.DiaSemana,
                    HoraAbertura = h.HoraAbertura,
                    HoraFechamento = h.HoraFechamento,
                    Fechado = h.Fechado
                }).ToList();
            }

            return response;
        }
    }
}