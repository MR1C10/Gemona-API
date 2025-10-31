using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Gemona.Domain.Entities;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly UserManager<Admin> _adminUserManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public SeedController(
            UserManager<Admin> adminUserManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _adminUserManager = adminUserManager;
            _roleManager = roleManager;
        }

        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            try
            {
                // Verificar se já existe
                var existingAdmin = await _adminUserManager.FindByEmailAsync(request.Email);
                if (existingAdmin != null)
                {
                    return BadRequest(new { success = false, message = "Admin já existe com este email" });
                }

                var existingByGithub = await _adminUserManager.Users
                    .FirstOrDefaultAsync(a => a.GithubUsername == request.GithubUsername);
                if (existingByGithub != null)
                {
                    return BadRequest(new { success = false, message = "Admin já existe com este username do GitHub" });
                }

                // Criar role Admin se não existir
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole<int>("Admin"));
                }

                // Criar admin
                var admin = new Admin
                {
                    Nome = request.Nome,
                    Email = request.Email,
                    UserName = request.Email,
                    GithubUsername = request.GithubUsername,
                    EmailConfirmed = true,
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                };

                var result = await _adminUserManager.CreateAsync(admin, request.Senha);
                if (!result.Succeeded)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Erro ao criar admin", 
                        errors = result.Errors.Select(e => e.Description) 
                    });
                }

                // Adicionar role
                await _adminUserManager.AddToRoleAsync(admin, "Admin");

                return Ok(new 
                { 
                    success = true, 
                    message = "Admin criado com sucesso",
                    data = new
                    {
                        id = admin.Id,
                        nome = admin.Nome,
                        email = admin.Email,
                        githubUsername = admin.GithubUsername
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    public class CreateAdminRequest
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string GithubUsername { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
