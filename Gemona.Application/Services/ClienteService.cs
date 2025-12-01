using Microsoft.AspNetCore.Identity;
using Gemona.Application.DTOs.Request.Cliente;
using Gemona.Application.DTOs.Response.Cliente;
using Gemona.Application.DTOs.Response.Endereco;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Exceptions;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.Application.Services
{
    public class ClienteService : IClienteService
    {
        private readonly UserManager<Cliente> _userManager;
        private readonly IClienteRepository _clienteRepository;
        private readonly IEnderecoRepository _enderecoRepository;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IGeocodingService _geocodingService;

        public ClienteService(
            UserManager<Cliente> userManager,
            IClienteRepository clienteRepository, 
            IEnderecoRepository enderecoRepository,
            IBlobStorageService blobStorageService,
            IGeocodingService geocodingService)
        {
            _userManager = userManager;
            _clienteRepository = clienteRepository;
            _enderecoRepository = enderecoRepository;
            _blobStorageService = blobStorageService;
            _geocodingService = geocodingService;
        }

        public async Task<ApiResponse<IEnumerable<ClienteResponse>>> GetAllAsync()
        {
            var clientes = await _clienteRepository.GetAllActiveWithEnderecosAsync();
            var response = clientes.Select(MapToResponseWithEndereco);
            
            return ApiResponse<IEnumerable<ClienteResponse>>.SuccessResult(
                response, "Clientes recuperados com sucesso");
        }

        public async Task<ApiResponse<ClienteResponse?>> GetByIdAsync(int id)
        {
            var cliente = await _clienteRepository.GetClienteWithEnderecoAsync(id);
            if (cliente == null)
            {
                throw new NotFoundException("Cliente", id);
            }

            var response = MapToResponseWithEndereco(cliente);
            return ApiResponse<ClienteResponse?>.SuccessResult(
                response, "Cliente encontrado com sucesso");
        }

        public async Task<ApiResponse<ClienteResponse?>> GetByEmailAsync(string email)
        {
            var cliente = await _clienteRepository.GetByEmailWithEnderecoAsync(email);
            if (cliente == null)
            {
                throw new NotFoundException($"Cliente com email '{email}' não foi encontrado");
            }

            var response = MapToResponseWithEndereco(cliente);
            return ApiResponse<ClienteResponse?>.SuccessResult(
                response, "Cliente encontrado com sucesso");
        }

        public async Task<ApiResponse<ClienteResponse?>> GetByCpfAsync(string cpf)
        {
            var cliente = await _clienteRepository.GetByCpfWithEnderecoAsync(cpf);
            if (cliente == null)
            {
                throw new NotFoundException($"Cliente com CPF '{cpf}' não foi encontrado");
            }

            var response = MapToResponseWithEndereco(cliente);
            return ApiResponse<ClienteResponse?>.SuccessResult(
                response, "Cliente encontrado com sucesso");
        }

        public async Task<ApiResponse<ClienteResponse?>> GetClienteWithEnderecoAsync(int clienteId)
        {
            var cliente = await _clienteRepository.GetClienteWithEnderecoAsync(clienteId);
            if (cliente == null)
            {
                throw new NotFoundException("Cliente", clienteId);
            }

            var response = MapToResponseWithEndereco(cliente);
            return ApiResponse<ClienteResponse?>.SuccessResult(
                response, "Cliente com endereço encontrado com sucesso");
        }

        public async Task<ApiResponse<IEnumerable<ClienteResponse>>> GetClientesByIdadeAsync(int idadeMinima, int idadeMaxima)
        {
            var clientes = await _clienteRepository.GetClientesByIdadeWithEnderecosAsync(idadeMinima, idadeMaxima);
            var response = clientes.Select(MapToResponseWithEndereco);
            
            return ApiResponse<IEnumerable<ClienteResponse>>.SuccessResult(
                response, $"Clientes entre {idadeMinima} e {idadeMaxima} anos encontrados");
        }

        public async Task<ApiResponse<ClienteResponse>> CreateAsync(CreateClienteRequest request)
        {
            // Validações usando UserManager
            var cpfValueObject = new Cpf(request.Cpf);
            
            var emailExists = await _userManager.FindByEmailAsync(request.Email);
            if (emailExists != null)
            {
                throw new BusinessException("Já existe um cliente com este email");
            }

            var cpfExists = await _clienteRepository.CpfExistsAsync(cpfValueObject);
            if (cpfExists)
            {
                throw new BusinessException("Já existe um cliente com este CPF");
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
            if (request.ImagemPerfil != null)
            {
                var imageBytes = Convert.FromBase64String(request.ImagemPerfil.Base64Data);
                using var imageStream = new MemoryStream(imageBytes);
                imagemUrl = await _blobStorageService.UploadImageAsync(
                    imageStream,
                    request.ImagemPerfil.FileName,
                    request.ImagemPerfil.ContentType
                );
            }

            // Criar cliente usando UserManager
            var cliente = new Cliente
            {
                UserName = request.Email, // Identity requer UserName
                Email = request.Email,
                PhoneNumber = request.Telefone, // Identity usa PhoneNumber
                Nome = request.Nome,
                Cpf = cpfValueObject,
                ImagemPerfilUrl = imagemUrl,
                DataNascimento = request.DataNascimento,
                DataCriacao = DateTime.UtcNow,
                EnderecoId = enderecoResult.EnderecoId
            };

            var result = await _userManager.CreateAsync(cliente, request.Senha);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BusinessException($"Erro ao criar cliente: {errors}");
            }

            var createdClienteWithEndereco = await _clienteRepository.GetClienteWithEnderecoAsync(cliente.Id);
            if (createdClienteWithEndereco == null)
            {
                throw new NotFoundException("Não foi possível encontrar o cliente recém-criado.");
            }

            var response = MapToResponseWithEndereco(createdClienteWithEndereco);
            return ApiResponse<ClienteResponse>.SuccessResult(
                response, "Cliente criado com sucesso");
        }

        public async Task<ApiResponse<ClienteResponse>> UpdateAsync(int id, UpdateClienteRequest request)
        {
            var cliente = await _userManager.FindByIdAsync(id.ToString());
            if (cliente == null)
            {
                throw new NotFoundException("Cliente", id);
            }

            // Verificar se novo email já existe (exceto no próprio cliente)
            var clienteExistente = await _userManager.FindByEmailAsync(request.Email);
            if (clienteExistente != null && clienteExistente.Id != id)
            {
                throw new BusinessException("Já existe um cliente com este email");
            }

            // Atualizar propriedades do Identity
            cliente.Email = request.Email;
            cliente.UserName = request.Email;
            cliente.PhoneNumber = request.Telefone;

            // Upload da nova imagem se fornecida
            if (request.ImagemPerfil != null)
            {
                // Deletar imagem antiga se existir
                if (!string.IsNullOrEmpty(cliente.ImagemPerfilUrl))
                {
                    await _blobStorageService.DeleteImageAsync(cliente.ImagemPerfilUrl);
                }

                // Upload da nova imagem
                var imageBytes = Convert.FromBase64String(request.ImagemPerfil.Base64Data);
                using var imageStream = new MemoryStream(imageBytes);
                cliente.ImagemPerfilUrl = await _blobStorageService.UploadImageAsync(
                    imageStream,
                    request.ImagemPerfil.FileName,
                    request.ImagemPerfil.ContentType
                );
            }

            // Atualizar propriedades customizadas
            cliente.Nome = request.Nome;
            cliente.DataNascimento = request.DataNascimento;
            cliente.DataAtualizacao = DateTime.UtcNow;

                // Atualizar endereço se existir
                if (cliente.EnderecoId.HasValue)
                {
                    var endereco = await _enderecoRepository.GetByIdAsync(cliente.EnderecoId.Value);
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
                }

            var result = await _userManager.UpdateAsync(cliente);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<ClienteResponse>.ErrorResult(
                    "Erro ao atualizar cliente", errors);
            }

            var response = MapToResponse(cliente);
            return ApiResponse<ClienteResponse>.SuccessResult(
                response, "Cliente atualizado com sucesso");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var cliente = await _userManager.FindByIdAsync(id.ToString());
            if (cliente == null)
            {
                throw new NotFoundException("Cliente", id);
            }

            // Soft delete
            cliente.Ativo = false;
            cliente.DataAtualizacao = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(cliente);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse<bool>.ErrorResult("Erro ao excluir cliente", errors);
            }

            return ApiResponse<bool>.SuccessResult(true, "Cliente excluído com sucesso");
        }

        public async Task<ApiResponse<bool>> EmailExistsAsync(string email)
        {
            var cliente = await _userManager.FindByEmailAsync(email);
            return ApiResponse<bool>.SuccessResult(
                cliente != null, "Verificação realizada com sucesso");
        }

        public async Task<ApiResponse<bool>> CpfExistsAsync(string cpf)
        {
            var cpfValueObject = new Cpf(cpf);
            var exists = await _clienteRepository.CpfExistsAsync(cpfValueObject);
            return ApiResponse<bool>.SuccessResult(exists, "Verificação realizada com sucesso");
        }

        private static ClienteResponse MapToResponse(Cliente cliente)
        {
            return new ClienteResponse
            {
                ClienteId = cliente.Id, // Identity usa Id
                Nome = cliente.Nome,
                Email = cliente.Email ?? string.Empty,
                Telefone = cliente.PhoneNumber ?? string.Empty, // Identity usa PhoneNumber
                Cpf = cliente.Cpf.Valor,
                ImagemPerfilUrl = cliente.ImagemPerfilUrl,
                DataNascimento = cliente.DataNascimento,
                DataCriacao = cliente.DataCriacao,
                DataAtualizacao = cliente.DataAtualizacao,
                Ativo = cliente.Ativo
            };
        }

        private static ClienteResponse MapToResponseWithEndereco(Cliente cliente)
        {
            var response = MapToResponse(cliente);
            
            if (cliente.Endereco != null)
            {
                response.Endereco = new EnderecoResponse
                {
                    EnderecoId = cliente.Endereco.EnderecoId,
                    Rua = cliente.Endereco.Rua,
                    Numero = cliente.Endereco.Numero,
                    Bairro = cliente.Endereco.Bairro,
                    Complemento = cliente.Endereco.Complemento,
                    Cidade = cliente.Endereco.Cidade,
                    Estado = cliente.Endereco.Estado,
                    Cep = cliente.Endereco.Cep.Valor,
                    Latitude = cliente.Endereco.Latitude,
                    Longitude = cliente.Endereco.Longitude,
                    DataCriacao = cliente.Endereco.DataCriacao,
                    DataAtualizacao = cliente.Endereco.DataAtualizacao,
                    Ativo = cliente.Endereco.Ativo
                };
            }

            return response;
        }
    }
}