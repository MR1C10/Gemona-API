using Microsoft.AspNetCore.Identity;
using Gemona.Application.DTOs.Request.Cliente;
using Gemona.Application.DTOs.Response.Cliente;
using Gemona.Application.DTOs.Response.Endereco;
using Gemona.Application.DTOs.Shared;
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

        public ClienteService(
            UserManager<Cliente> userManager,
            IClienteRepository clienteRepository, 
            IEnderecoRepository enderecoRepository)
        {
            _userManager = userManager;
            _clienteRepository = clienteRepository;
            _enderecoRepository = enderecoRepository;
        }

        public async Task<ApiResponse<IEnumerable<ClienteResponse>>> GetAllAsync()
        {
            try
            {
                var clientes = await _clienteRepository.GetAllActiveAsync();
                var response = clientes.Select(MapToResponse);
                
                return ApiResponse<IEnumerable<ClienteResponse>>.SuccessResult(
                    response, "Clientes recuperados com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ClienteResponse>>.ErrorResult(
                    "Erro ao buscar clientes", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ClienteResponse?>> GetByIdAsync(int id)
        {
            try
            {
                var cliente = await _userManager.FindByIdAsync(id.ToString());
                if (cliente == null)
                {
                    return ApiResponse<ClienteResponse?>.ErrorResult("Cliente não encontrado");
                }

                var response = MapToResponse(cliente);
                return ApiResponse<ClienteResponse?>.SuccessResult(
                    response, "Cliente encontrado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClienteResponse?>.ErrorResult(
                    "Erro ao buscar cliente", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ClienteResponse?>> GetByEmailAsync(string email)
        {
            try
            {
                var cliente = await _userManager.FindByEmailAsync(email);
                if (cliente == null)
                {
                    return ApiResponse<ClienteResponse?>.ErrorResult("Cliente não encontrado");
                }

                var response = MapToResponse(cliente);
                return ApiResponse<ClienteResponse?>.SuccessResult(
                    response, "Cliente encontrado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClienteResponse?>.ErrorResult(
                    "Erro ao buscar cliente", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ClienteResponse?>> GetByCpfAsync(string cpf)
        {
            try
            {
                var cpfValueObject = new Cpf(cpf);
                var cliente = await _clienteRepository.GetByCpfAsync(cpfValueObject);
                if (cliente == null)
                {
                    return ApiResponse<ClienteResponse?>.ErrorResult("Cliente não encontrado");
                }

                var response = MapToResponse(cliente);
                return ApiResponse<ClienteResponse?>.SuccessResult(
                    response, "Cliente encontrado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClienteResponse?>.ErrorResult(
                    "Erro ao buscar cliente", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ClienteResponse?>> GetClienteWithEnderecoAsync(int clienteId)
        {
            try
            {
                var cliente = await _clienteRepository.GetClienteWithEnderecoAsync(clienteId);
                if (cliente == null)
                {
                    return ApiResponse<ClienteResponse?>.ErrorResult("Cliente não encontrado");
                }

                var response = MapToResponseWithEndereco(cliente);
                return ApiResponse<ClienteResponse?>.SuccessResult(
                    response, "Cliente com endereço encontrado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClienteResponse?>.ErrorResult(
                    "Erro ao buscar cliente", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<IEnumerable<ClienteResponse>>> GetClientesByIdadeAsync(int idadeMinima, int idadeMaxima)
        {
            try
            {
                var clientes = await _clienteRepository.GetClientesByIdadeAsync(idadeMinima, idadeMaxima);
                var response = clientes.Select(MapToResponse);
                
                return ApiResponse<IEnumerable<ClienteResponse>>.SuccessResult(
                    response, $"Clientes entre {idadeMinima} e {idadeMaxima} anos encontrados");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ClienteResponse>>.ErrorResult(
                    "Erro ao buscar clientes por idade", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ClienteResponse>> CreateAsync(CreateClienteRequest request)
        {
            try
            {
                // Validações usando UserManager
                var cpfValueObject = new Cpf(request.Cpf);
                
                var emailExists = await _userManager.FindByEmailAsync(request.Email);
                if (emailExists != null)
                {
                    return ApiResponse<ClienteResponse>.ErrorResult("Já existe um cliente com este email");
                }

                var cpfExists = await _clienteRepository.CpfExistsAsync(cpfValueObject);
                if (cpfExists)
                {
                    return ApiResponse<ClienteResponse>.ErrorResult("Já existe um cliente com este CPF");
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
                    Latitude = request.Latitude,
                    Longitude = request.Longitude
                };

                var enderecoResult = await _enderecoRepository.AddAsync(endereco);
                await _enderecoRepository.SaveChangesAsync();

                // Criar cliente usando UserManager
                var cliente = new Cliente
                {
                    UserName = request.Email, // Identity requer UserName
                    Email = request.Email,
                    PhoneNumber = request.Telefone, // Identity usa PhoneNumber
                    Nome = request.Nome,
                    Cpf = cpfValueObject,
                    ImagemPerfilUrl = request.ImagemPerfilUrl,
                    DataNascimento = request.DataNascimento,
                    DataCriacao = DateTime.UtcNow,
                    EnderecoId = enderecoResult.EnderecoId
                };

                var result = await _userManager.CreateAsync(cliente, request.Senha);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<ClienteResponse>.ErrorResult(
                        "Erro ao criar cliente", errors);
                }

                var response = MapToResponse(cliente);
                return ApiResponse<ClienteResponse>.SuccessResult(
                    response, "Cliente criado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<ClienteResponse>.ErrorResult(
                    "Erro ao criar cliente", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ClienteResponse>> UpdateAsync(int id, UpdateClienteRequest request)
        {
            try
            {
                var cliente = await _userManager.FindByIdAsync(id.ToString());
                if (cliente == null)
                {
                    return ApiResponse<ClienteResponse>.ErrorResult("Cliente não encontrado");
                }

                // Verificar se novo email já existe (exceto no próprio cliente)
                var clienteExistente = await _userManager.FindByEmailAsync(request.Email);
                if (clienteExistente != null && clienteExistente.Id != id)
                {
                    return ApiResponse<ClienteResponse>.ErrorResult("Já existe um cliente com este email");
                }

                // Atualizar propriedades do Identity
                cliente.Email = request.Email;
                cliente.UserName = request.Email;
                cliente.PhoneNumber = request.Telefone;

                // Atualizar propriedades customizadas
                cliente.Nome = request.Nome;
                cliente.ImagemPerfilUrl = request.ImagemPerfilUrl;
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
            catch (Exception ex)
            {
                return ApiResponse<ClienteResponse>.ErrorResult(
                    "Erro ao atualizar cliente", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var cliente = await _userManager.FindByIdAsync(id.ToString());
                if (cliente == null)
                {
                    return ApiResponse<bool>.ErrorResult("Cliente não encontrado");
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
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResult(
                    "Erro ao excluir cliente", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> EmailExistsAsync(string email)
        {
            try
            {
                var cliente = await _userManager.FindByEmailAsync(email);
                return ApiResponse<bool>.SuccessResult(
                    cliente != null, "Verificação realizada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResult(
                    "Erro ao verificar email", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> CpfExistsAsync(string cpf)
        {
            try
            {
                var cpfValueObject = new Cpf(cpf);
                var exists = await _clienteRepository.CpfExistsAsync(cpfValueObject);
                return ApiResponse<bool>.SuccessResult(exists, "Verificação realizada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResult(
                    "Erro ao verificar CPF", new List<string> { ex.Message });
            }
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