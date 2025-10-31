using Microsoft.AspNetCore.Identity;
using Gemona.Application.DTOs.Request.Profissional;
using Gemona.Application.DTOs.Response.Profissional;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;
using Gemona.Application.DTOs.Response.Estabelecimento;

namespace Gemona.Application.Services
{
    public class ProfissionalService : IProfissionalService
    {
        private readonly UserManager<Profissional> _userManager;
        private readonly IProfissionalRepository _profissionalRepository;
        private readonly IEstabelecimentoRepository _estabelecimentoRepository;

        public ProfissionalService(
            UserManager<Profissional> userManager,
            IProfissionalRepository profissionalRepository,
            IEstabelecimentoRepository estabelecimentoRepository)
        {
            _userManager = userManager;
            _profissionalRepository = profissionalRepository;
            _estabelecimentoRepository = estabelecimentoRepository;
        }

        public async Task<ApiResponse<IEnumerable<ProfissionalResponse>>> GetAllAsync()
        {
            try
            {
                var profissionais = await _profissionalRepository.GetAllActiveAsync();
                var response = profissionais.Select(MapToResponse);
                
                return ApiResponse<IEnumerable<ProfissionalResponse>>.SuccessResult(
                    response, "Profissionais recuperados com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProfissionalResponse>>.ErrorResult(
                    "Erro ao buscar profissionais", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ProfissionalResponse?>> GetByIdAsync(int id)
        {
            try
            {
                var profissional = await _userManager.FindByIdAsync(id.ToString());
                if (profissional == null)
                {
                    return ApiResponse<ProfissionalResponse?>.ErrorResult("Profissional não encontrado");
                }

                var response = MapToResponse(profissional);
                return ApiResponse<ProfissionalResponse?>.SuccessResult(
                    response, "Profissional encontrado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfissionalResponse?>.ErrorResult(
                    "Erro ao buscar profissional", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ProfissionalResponse?>> GetByEmailAsync(string email)
        {
            try
            {
                var profissional = await _userManager.FindByEmailAsync(email);
                if (profissional == null)
                {
                    return ApiResponse<ProfissionalResponse?>.ErrorResult("Profissional não encontrado");
                }

                var response = MapToResponse(profissional);
                return ApiResponse<ProfissionalResponse?>.SuccessResult(
                    response, "Profissional encontrado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfissionalResponse?>.ErrorResult(
                    "Erro ao buscar profissional", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ProfissionalResponse?>> GetByCpfAsync(string cpf)
        {
            try
            {
                var cpfValueObject = new Cpf(cpf);
                var profissional = await _profissionalRepository.GetByCpfAsync(cpfValueObject);
                if (profissional == null)
                {
                    return ApiResponse<ProfissionalResponse?>.ErrorResult("Profissional não encontrado");
                }

                var response = MapToResponse(profissional);
                return ApiResponse<ProfissionalResponse?>.SuccessResult(
                    response, "Profissional encontrado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfissionalResponse?>.ErrorResult(
                    "Erro ao buscar profissional", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ProfissionalWithEstabelecimentoResponse?>> GetProfissionalWithEstabelecimentoAsync(int profissionalId)
        {
            try
            {
                var profissional = await _profissionalRepository.GetProfissionalWithEstabelecimentoAsync(profissionalId);
                if (profissional == null)
                {
                    return ApiResponse<ProfissionalWithEstabelecimentoResponse?>.ErrorResult("Profissional não encontrado");
                }

                var response = MapToResponseWithEstabelecimento(profissional);
                return ApiResponse<ProfissionalWithEstabelecimentoResponse?>.SuccessResult(
                    response, "Profissional com estabelecimento encontrado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfissionalWithEstabelecimentoResponse?>.ErrorResult(
                    "Erro ao buscar profissional", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ProfissionalResponse>> CreateAsync(CreateProfissionalRequest request)
        {
            try
            {
                // Validações usando UserManager
                var cpfValueObject = new Cpf(request.Cpf);
                
                var emailExists = await _userManager.FindByEmailAsync(request.Email);
                if (emailExists != null)
                {
                    return ApiResponse<ProfissionalResponse>.ErrorResult("Já existe um profissional com este email");
                }

                var cpfExists = await _profissionalRepository.CpfExistsAsync(cpfValueObject);
                if (cpfExists)
                {
                    return ApiResponse<ProfissionalResponse>.ErrorResult("Já existe um profissional com este CPF");
                }

                // Criar profissional usando UserManager
                var profissional = new Profissional
                {
                    UserName = request.Email, // Identity requer UserName
                    Email = request.Email,
                    PhoneNumber = request.Telefone, // Identity usa PhoneNumber
                    Nome = request.Nome,
                    Cpf = cpfValueObject,
                    ImagemPerfilUrl = request.ImagemPerfilUrl,
                    DataNascimento = request.DataNascimento,
                    DataCriacao = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(profissional, request.Senha);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<ProfissionalResponse>.ErrorResult(
                        "Erro ao criar profissional", errors);
                }

                var response = MapToResponse(profissional);
                return ApiResponse<ProfissionalResponse>.SuccessResult(
                    response, "Profissional criado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfissionalResponse>.ErrorResult(
                    "Erro ao criar profissional", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<ProfissionalResponse>> UpdateAsync(int id, UpdateProfissionalRequest request)
        {
            try
            {
                var profissional = await _userManager.FindByIdAsync(id.ToString());
                if (profissional == null)
                {
                    return ApiResponse<ProfissionalResponse>.ErrorResult("Profissional não encontrado");
                }

                // Verificar se novo email já existe (exceto no próprio profissional)
                var profissionalExistente = await _userManager.FindByEmailAsync(request.Email);
                if (profissionalExistente != null && profissionalExistente.Id != id)
                {
                    return ApiResponse<ProfissionalResponse>.ErrorResult("Já existe um profissional com este email");
                }

                // Atualizar propriedades do Identity
                profissional.Email = request.Email;
                profissional.UserName = request.Email;
                profissional.PhoneNumber = request.Telefone;

                // Atualizar propriedades customizadas
                profissional.Nome = request.Nome;
                profissional.ImagemPerfilUrl = request.ImagemPerfilUrl;
                profissional.DataNascimento = request.DataNascimento;
                profissional.DataAtualizacao = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(profissional);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<ProfissionalResponse>.ErrorResult(
                        "Erro ao atualizar profissional", errors);
                }

                var response = MapToResponse(profissional);
                return ApiResponse<ProfissionalResponse>.SuccessResult(
                    response, "Profissional atualizado com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProfissionalResponse>.ErrorResult(
                    "Erro ao atualizar profissional", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            try
            {
                var profissional = await _userManager.FindByIdAsync(id.ToString());
                if (profissional == null)
                {
                    return ApiResponse<bool>.ErrorResult("Profissional não encontrado");
                }

                // Soft delete
                profissional.Ativo = false;
                profissional.DataAtualizacao = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(profissional);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return ApiResponse<bool>.ErrorResult("Erro ao excluir profissional", errors);
                }

                return ApiResponse<bool>.SuccessResult(true, "Profissional excluído com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResult(
                    "Erro ao excluir profissional", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> EmailExistsAsync(string email)
        {
            try
            {
                var profissional = await _userManager.FindByEmailAsync(email);
                return ApiResponse<bool>.SuccessResult(
                    profissional != null, "Verificação realizada com sucesso");
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
                var exists = await _profissionalRepository.CpfExistsAsync(cpfValueObject);
                return ApiResponse<bool>.SuccessResult(exists, "Verificação realizada com sucesso");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResult(
                    "Erro ao verificar CPF", new List<string> { ex.Message });
            }
        }

        private static ProfissionalResponse MapToResponse(Profissional profissional)
        {
            return new ProfissionalResponse
            {
                ProfissionalId = profissional.Id, // Identity usa Id
                Nome = profissional.Nome,
                Email = profissional.Email ?? string.Empty,
                Telefone = profissional.PhoneNumber ?? string.Empty, // Identity usa PhoneNumber
                Cpf = profissional.Cpf.Valor,
                ImagemPerfilUrl = profissional.ImagemPerfilUrl,
                DataNascimento = profissional.DataNascimento,
                DataCriacao = profissional.DataCriacao,
                DataAtualizacao = profissional.DataAtualizacao,
                Ativo = profissional.Ativo
            };
        }

        private static ProfissionalWithEstabelecimentoResponse MapToResponseWithEstabelecimento(Profissional profissional)
        {
            var response = new ProfissionalWithEstabelecimentoResponse
            {
                ProfissionalId = profissional.Id,
                Nome = profissional.Nome,
                Email = profissional.Email ?? string.Empty,
                Telefone = profissional.PhoneNumber ?? string.Empty,
                Cpf = profissional.Cpf.Valor,
                ImagemPerfilUrl = profissional.ImagemPerfilUrl,
                DataNascimento = profissional.DataNascimento,
                DataCriacao = profissional.DataCriacao,
                DataAtualizacao = profissional.DataAtualizacao,
                Ativo = profissional.Ativo
            };

            if (profissional.Estabelecimento != null)
            {
                response.Estabelecimento = new EstabelecimentoResponse
                {
                    EstabelecimentoId = profissional.Estabelecimento.EstabelecimentoId,
                    Nome = profissional.Estabelecimento.Nome,
                    Cnpj = profissional.Estabelecimento.Cnpj.Valor,
                    Telefone = profissional.Estabelecimento.Telefone,
                    Email = profissional.Estabelecimento.Email,
                    ImagemEstabelecimentoUrl = profissional.Estabelecimento.ImagemEstabelecimentoUrl,
                    DataCriacao = profissional.Estabelecimento.DataCriacao,
                    DataAtualizacao = profissional.Estabelecimento.DataAtualizacao,
                    Ativo = profissional.Estabelecimento.Ativo,
                    ProfissionalId = profissional.Estabelecimento.ProfissionalId
                };
            }

            return response;
        }
    }
}