using Microsoft.AspNetCore.Identity;
using Gemona.Application.DTOs.Request.Cliente;
using Gemona.Application.DTOs.Request.Profissional;
using Gemona.Application.DTOs.Shared;
using Gemona.Application.Interfaces.Services;
using Gemona.Domain.Entities;
using Gemona.Application.Exceptions;

namespace Gemona.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Cliente> _clienteUserManager;
        private readonly UserManager<Profissional> _profissionalUserManager;
        private readonly UserManager<Admin> _adminUserManager;
        private readonly IJwtService _jwtService;

        public AuthService(
            UserManager<Cliente> clienteUserManager,
            UserManager<Profissional> profissionalUserManager,
            UserManager<Admin> adminUserManager,
            IJwtService jwtService)
        {
            _clienteUserManager = clienteUserManager;
            _profissionalUserManager = profissionalUserManager;
            _adminUserManager = adminUserManager;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse<LoginResponse>> LoginClienteAsync(LoginClienteRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            {
                throw new BusinessException("Email e senha são obrigatórios");
            }

            var cliente = await _clienteUserManager.FindByEmailAsync(request.Email);
            if (cliente == null)
            {
                throw new UnauthorizedException("Email ou senha incorretos");
            }

            if (!cliente.Ativo)
            {
                throw new UnauthorizedException("Usuário inativo");
            }

            var result = await _clienteUserManager.CheckPasswordAsync(cliente, request.Senha);
            if (!result)
            {
                throw new UnauthorizedException("Email ou senha incorretos");
            }

            var token = _jwtService.GenerateTokenForCliente(cliente);
            var expiresAt = DateTime.UtcNow.AddDays(7); // Deve corresponder ao ExpireDays do appsettings

            var response = new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserType = "Cliente",
                UserId = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email ?? string.Empty
            };

            return ApiResponse<LoginResponse>.SuccessResult(response, "Login realizado com sucesso");
        }

        public async Task<ApiResponse<LoginResponse>> LoginProfissionalAsync(LoginProfissionalRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Senha))
            {
                throw new BusinessException("Email e senha são obrigatórios");
            }

            var profissional = await _profissionalUserManager.FindByEmailAsync(request.Email);
            if (profissional == null)
            {
                throw new UnauthorizedException("Email ou senha incorretos");
            }

            if (!profissional.Ativo)
            {
                throw new UnauthorizedException("Usuário inativo");
            }

            var result = await _profissionalUserManager.CheckPasswordAsync(profissional, request.Senha);
            if (!result)
            {
                throw new UnauthorizedException("Email ou senha incorretos");
            }

            var token = _jwtService.GenerateTokenForProfissional(profissional);
            var expiresAt = DateTime.UtcNow.AddDays(7); // Deve corresponder ao ExpireDays do appsettings

            var response = new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserType = "Profissional",
                UserId = profissional.Id,
                Nome = profissional.Nome,
                Email = profissional.Email ?? string.Empty
            };

            return ApiResponse<LoginResponse>.SuccessResult(response, "Login realizado com sucesso");
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                throw new BusinessException("Email e senha são obrigatórios");
            }

            // Tentar login como cliente primeiro
            var cliente = await _clienteUserManager.FindByEmailAsync(email);
            if (cliente != null && cliente.Ativo)
            {
                var clienteResult = await _clienteUserManager.CheckPasswordAsync(cliente, senha);
                if (clienteResult)
                {
                    var token = _jwtService.GenerateTokenForCliente(cliente);
                    var expiresAt = DateTime.UtcNow.AddDays(7);

                    var response = new LoginResponse
                    {
                        Token = token,
                        ExpiresAt = expiresAt,
                        UserType = "Cliente",
                        UserId = cliente.Id,
                        Nome = cliente.Nome,
                        Email = cliente.Email ?? string.Empty
                    };

                    return ApiResponse<LoginResponse>.SuccessResult(response, "Login realizado com sucesso");
                }
            }

            // Tentar login como profissional
            var profissional = await _profissionalUserManager.FindByEmailAsync(email);
            if (profissional != null && profissional.Ativo)
            {
                var profissionalResult = await _profissionalUserManager.CheckPasswordAsync(profissional, senha);
                if (profissionalResult)
                {
                    var token = _jwtService.GenerateTokenForProfissional(profissional);
                    var expiresAt = DateTime.UtcNow.AddDays(7);

                    var response = new LoginResponse
                    {
                        Token = token,
                        ExpiresAt = expiresAt,
                        UserType = "Profissional",
                        UserId = profissional.Id,
                        Nome = profissional.Nome,
                        Email = profissional.Email ?? string.Empty
                    };

                    return ApiResponse<LoginResponse>.SuccessResult(response, "Login realizado com sucesso");
                }
            }

            // Tentar login como admin
            var admin = await _adminUserManager.FindByEmailAsync(email);
            if (admin != null && admin.Ativo)
            {
                var adminResult = await _adminUserManager.CheckPasswordAsync(admin, senha);
                if (adminResult)
                {
                    var token = _jwtService.GenerateTokenForAdmin(admin);
                    var expiresAt = DateTime.UtcNow.AddDays(7);

                    var response = new LoginResponse
                    {
                        Token = token,
                        ExpiresAt = expiresAt,
                        UserType = "Admin",
                        UserId = admin.Id,
                        Nome = admin.Nome,
                        Email = admin.Email ?? string.Empty
                    };

                    return ApiResponse<LoginResponse>.SuccessResult(response, "Login realizado com sucesso");
                }
            }

            throw new UnauthorizedException("Email ou senha incorretos");
        }

        public async Task<ApiResponse<bool>> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new BusinessException("Token não fornecido");
            }

            var isValid = _jwtService.ValidateToken(token);
            
            if (!isValid)
            {
                throw new UnauthorizedException("Token inválido ou expirado");
            }

            // Verificar se o usuário ainda está ativo
            var userId = _jwtService.GetUserIdFromToken(token);
            var userType = _jwtService.GetUserTypeFromToken(token);

            if (userId.HasValue && !string.IsNullOrEmpty(userType))
            {
                if (userType == "Cliente")
                {
                    var cliente = await _clienteUserManager.FindByIdAsync(userId.Value.ToString());
                    if (cliente == null || !cliente.Ativo)
                    {
                        throw new UnauthorizedException("Usuário não encontrado ou inativo");
                    }
                }
                else if (userType == "Profissional")
                {
                    var profissional = await _profissionalUserManager.FindByIdAsync(userId.Value.ToString());
                    if (profissional == null || !profissional.Ativo)
                    {
                        throw new UnauthorizedException("Usuário não encontrado ou inativo");
                    }
                }
            }

            return ApiResponse<bool>.SuccessResult(true, "Token válido");
        }

        public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new BusinessException("Token não fornecido");
            }

            var isValid = _jwtService.ValidateToken(token);
            if (!isValid)
            {
                throw new UnauthorizedException("Token inválido ou expirado");
            }

            var userId = _jwtService.GetUserIdFromToken(token);
            var userType = _jwtService.GetUserTypeFromToken(token);

            if (!userId.HasValue || string.IsNullOrEmpty(userType))
            {
                throw new UnauthorizedException("Token inválido");
            }

            if (userType == "Cliente")
            {
                var cliente = await _clienteUserManager.FindByIdAsync(userId.Value.ToString());
                if (cliente == null || !cliente.Ativo)
                {
                    throw new UnauthorizedException("Usuário não encontrado ou inativo");
                }

                var newToken = _jwtService.GenerateTokenForCliente(cliente);
                var expiresAt = DateTime.UtcNow.AddDays(7);

                var response = new LoginResponse
                {
                    Token = newToken,
                    ExpiresAt = expiresAt,
                    UserType = "Cliente",
                    UserId = cliente.Id,
                    Nome = cliente.Nome,
                    Email = cliente.Email ?? string.Empty
                };

                return ApiResponse<LoginResponse>.SuccessResult(response, "Token renovado com sucesso");
            }
            else if (userType == "Profissional")
            {
                var profissional = await _profissionalUserManager.FindByIdAsync(userId.Value.ToString());
                if (profissional == null || !profissional.Ativo)
                {
                    throw new UnauthorizedException("Usuário não encontrado ou inativo");
                }

                var newToken = _jwtService.GenerateTokenForProfissional(profissional);
                var expiresAt = DateTime.UtcNow.AddDays(7);

                var response = new LoginResponse
                {
                    Token = newToken,
                    ExpiresAt = expiresAt,
                    UserType = "Profissional",
                    UserId = profissional.Id,
                    Nome = profissional.Nome,
                    Email = profissional.Email ?? string.Empty
                };

                return ApiResponse<LoginResponse>.SuccessResult(response, "Token renovado com sucesso");
            }
            else if (userType == "Admin")
            {
                var admin = await _adminUserManager.FindByIdAsync(userId.Value.ToString());
                if (admin == null || !admin.Ativo)
                {
                    throw new UnauthorizedException("Usuário não encontrado ou inativo");
                }

                var newToken = _jwtService.GenerateTokenForAdmin(admin);
                var expiresAt = DateTime.UtcNow.AddDays(7);

                var response = new LoginResponse
                {
                    Token = newToken,
                    ExpiresAt = expiresAt,
                    UserType = "Admin",
                    UserId = admin.Id,
                    Nome = admin.Nome,
                    Email = admin.Email ?? string.Empty
                };

                return ApiResponse<LoginResponse>.SuccessResult(response, "Token renovado com sucesso");
            }

            throw new BusinessException("Tipo de usuário inválido");
        }

        public async Task<ApiResponse<LoginResponse>> LoginAdminAsync(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                throw new BusinessException("Email e senha são obrigatórios");
            }

            var admin = await _adminUserManager.FindByEmailAsync(email);
            if (admin == null)
            {
                throw new UnauthorizedException("Email ou senha incorretos");
            }

            if (!admin.Ativo)
            {
                throw new UnauthorizedException("Usuário inativo");
            }

            var result = await _adminUserManager.CheckPasswordAsync(admin, senha);
            if (!result)
            {
                throw new UnauthorizedException("Email ou senha incorretos");
            }

            var token = _jwtService.GenerateTokenForAdmin(admin);
            var expiresAt = DateTime.UtcNow.AddDays(7);

            var response = new LoginResponse
            {
                Token = token,
                ExpiresAt = expiresAt,
                UserType = "Admin",
                UserId = admin.Id,
                Nome = admin.Nome,
                Email = admin.Email ?? string.Empty
            };

            return ApiResponse<LoginResponse>.SuccessResult(response, "Login realizado com sucesso");
        }
    }
}
