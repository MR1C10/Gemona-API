using Microsoft.AspNetCore.Identity;
using Gemona.Application.DTOs.Request.Profissional;
using Gemona.Application.DTOs.Response.Profissional;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;
using Gemona.Application.DTOs.Response.Estabelecimento;
using Gemona.Application.Exceptions;

namespace Gemona.Application.Services
{
    public class ProfissionalService : IProfissionalService
    {
        private readonly UserManager<Profissional> _userManager;
        private readonly IProfissionalRepository _profissionalRepository;
        private readonly IEstabelecimentoRepository _estabelecimentoRepository;
        private readonly IBlobStorageService _blobStorageService;

        public ProfissionalService(
            UserManager<Profissional> userManager,
            IProfissionalRepository profissionalRepository,
            IEstabelecimentoRepository estabelecimentoRepository,
            IBlobStorageService blobStorageService)
        {
            _userManager = userManager;
            _profissionalRepository = profissionalRepository;
            _estabelecimentoRepository = estabelecimentoRepository;
            _blobStorageService = blobStorageService;
        }

        public async Task<ApiResponse<IEnumerable<ProfissionalResponse>>> GetAllAsync()
        {
            var profissionais = await _profissionalRepository.GetAllActiveAsync();
            var response = profissionais.Select(MapToResponse);
            
            return ApiResponse<IEnumerable<ProfissionalResponse>>.SuccessResult(
                response, "Profissionais recuperados com sucesso");
        }

        public async Task<ApiResponse<ProfissionalResponse?>> GetByIdAsync(int id)
        {
            var profissional = await _userManager.FindByIdAsync(id.ToString());
            if (profissional == null)
            {
                throw new NotFoundException("Profissional", id);
            }

            var response = MapToResponse(profissional);
            return ApiResponse<ProfissionalResponse?>.SuccessResult(
                response, "Profissional encontrado com sucesso");
        }

        public async Task<ApiResponse<ProfissionalResponse?>> GetByEmailAsync(string email)
        {
            var profissional = await _userManager.FindByEmailAsync(email);
            if (profissional == null)
            {
                throw new NotFoundException($"Profissional com email '{email}' não foi encontrado");
            }

            var response = MapToResponse(profissional);
            return ApiResponse<ProfissionalResponse?>.SuccessResult(
                response, "Profissional encontrado com sucesso");
        }

        public async Task<ApiResponse<ProfissionalResponse?>> GetByCpfAsync(string cpf)
        {
            var cpfValueObject = new Cpf(cpf);
            var profissional = await _profissionalRepository.GetByCpfAsync(cpfValueObject);
            if (profissional == null)
            {
                throw new NotFoundException($"Profissional com CPF '{cpf}' não foi encontrado");
            }

            var response = MapToResponse(profissional);
            return ApiResponse<ProfissionalResponse?>.SuccessResult(
                response, "Profissional encontrado com sucesso");
        }

        public async Task<ApiResponse<ProfissionalWithEstabelecimentoResponse?>> GetProfissionalWithEstabelecimentoAsync(int profissionalId)
        {
            var profissional = await _profissionalRepository.GetProfissionalWithEstabelecimentoAsync(profissionalId);
            if (profissional == null)
            {
                throw new NotFoundException("Profissional", profissionalId);
            }

            var response = MapToResponseWithEstabelecimento(profissional);
            return ApiResponse<ProfissionalWithEstabelecimentoResponse?>.SuccessResult(
                response, "Profissional com estabelecimento encontrado com sucesso");
        }

        public async Task<ApiResponse<ProfissionalResponse>> CreateAsync(CreateProfissionalRequest request)
        {
            // Validações usando UserManager
            var cpfValueObject = new Cpf(request.Cpf);
            
            var emailExists = await _userManager.FindByEmailAsync(request.Email);
            if (emailExists != null)
            {
                throw new BusinessException("Já existe um profissional com este email");
            }

            var cpfExists = await _profissionalRepository.CpfExistsAsync(cpfValueObject);
            if (cpfExists)
            {
                throw new BusinessException("Já existe um profissional com este CPF");
            }

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

            // Criar profissional usando UserManager
            var profissional = new Profissional
            {
                UserName = request.Email, // Identity requer UserName
                Email = request.Email,
                PhoneNumber = request.Telefone, // Identity usa PhoneNumber
                Nome = request.Nome,
                Cpf = cpfValueObject,
                ImagemPerfilUrl = imagemUrl,
                DataNascimento = request.DataNascimento,
                DataCriacao = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(profissional, request.Senha);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BusinessException($"Erro ao criar profissional: {errors}");
            }

            var response = MapToResponse(profissional);
            return ApiResponse<ProfissionalResponse>.SuccessResult(
                response, "Profissional criado com sucesso");
        }

        public async Task<ApiResponse<ProfissionalResponse>> UpdateAsync(int id, UpdateProfissionalRequest request)
        {
            var profissional = await _userManager.FindByIdAsync(id.ToString());
            if (profissional == null)
            {
                throw new NotFoundException("Profissional", id);
            }

            // Verificar se novo email já existe (exceto no próprio profissional)
            var profissionalExistente = await _userManager.FindByEmailAsync(request.Email);
            if (profissionalExistente != null && profissionalExistente.Id != id)
            {
                throw new BusinessException("Já existe um profissional com este email");
            }

            // Atualizar propriedades do Identity
            profissional.Email = request.Email;
            profissional.UserName = request.Email;
            profissional.PhoneNumber = request.Telefone;

            // Upload da nova imagem se fornecida
            if (request.ImagemPerfil != null)
            {
                // Deletar imagem antiga se existir
                if (!string.IsNullOrEmpty(profissional.ImagemPerfilUrl))
                {
                    await _blobStorageService.DeleteImageAsync(profissional.ImagemPerfilUrl);
                }

                // Upload da nova imagem
                var imageBytes = Convert.FromBase64String(request.ImagemPerfil.Base64Data);
                using var imageStream = new MemoryStream(imageBytes);
                profissional.ImagemPerfilUrl = await _blobStorageService.UploadImageAsync(
                    imageStream,
                    request.ImagemPerfil.FileName,
                    request.ImagemPerfil.ContentType
                );
            }

            // Atualizar propriedades customizadas
            profissional.Nome = request.Nome;
            profissional.DataNascimento = request.DataNascimento;
            profissional.DataAtualizacao = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(profissional);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BusinessException($"Erro ao atualizar profissional: {errors}");
            }

            var response = MapToResponse(profissional);
            return ApiResponse<ProfissionalResponse>.SuccessResult(
                response, "Profissional atualizado com sucesso");
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var profissional = await _userManager.FindByIdAsync(id.ToString());
            if (profissional == null)
            {
                throw new NotFoundException("Profissional", id);
            }

            // Soft delete
            profissional.Ativo = false;
            profissional.DataAtualizacao = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(profissional);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new BusinessException($"Erro ao excluir profissional: {errors}");
            }

            return ApiResponse<bool>.SuccessResult(true, "Profissional excluído com sucesso");
        }

        public async Task<ApiResponse<bool>> EmailExistsAsync(string email)
        {
            var profissional = await _userManager.FindByEmailAsync(email);
            return ApiResponse<bool>.SuccessResult(
                profissional != null, "Verificação realizada com sucesso");
        }

        public async Task<ApiResponse<bool>> CpfExistsAsync(string cpf)
        {
            var cpfValueObject = new Cpf(cpf);
            var exists = await _profissionalRepository.CpfExistsAsync(cpfValueObject);
            return ApiResponse<bool>.SuccessResult(exists, "Verificação realizada com sucesso");
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