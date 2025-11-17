using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Gemona.Domain.Entities;
using Gemona.Application.Interfaces.Services;
using Gemona.Application.DTOs.Request.Categoria;
using Gemona.Application.DTOs.Request.SubCategoria;

namespace Gemona.API.Controllers
{
    /// <summary>
    /// Controller para seed de dados iniciais (APENAS DESENVOLVIMENTO)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    #if !DEBUG
    [Authorize(Roles = "Admin")] // Em produção, apenas admins existentes podem criar novos
    #endif
    public class SeedController : ControllerBase
    {
        private readonly UserManager<Admin> _adminUserManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ILogger<SeedController> _logger;
        private readonly ICategoriaService _categoriaService;
        private readonly ISubCategoriaService _subCategoriaService;

        public SeedController(
            UserManager<Admin> adminUserManager,
            RoleManager<IdentityRole<int>> roleManager,
            ILogger<SeedController> logger,
            ICategoriaService categoriaService,
            ISubCategoriaService subCategoriaService)
        {
            _adminUserManager = adminUserManager;
            _roleManager = roleManager;
            _logger = logger;
            _categoriaService = categoriaService;
            _subCategoriaService = subCategoriaService;
        }

        /// <summary>
        /// Cria um usuário Admin (protegido em produção)
        /// </summary>
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminRequest request)
        {
            #if DEBUG
            _logger.LogWarning("AVISO: Endpoint de criação de admin exposto em modo DEBUG");
            #endif
            
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

        /// <summary>
        /// Cria categorias base do sistema (protegido em produção)
        /// </summary>
        [HttpPost("seed-categorias")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SeedCategorias()
        {
            try
            {
                // Verificar se já existem categorias
                var existingCategories = await _categoriaService.GetAllAsync();
                if (existingCategories.Success && existingCategories.Data?.Any() == true)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Categorias já existem no sistema.",
                        count = existingCategories.Data.Count()
                    });
                }

                // Categorias base do sistema
                var categorias = new List<string>
                {
                    "Beleza e Estética",
                    "Saúde e Bem-Estar",
                    "Casa e Manutenção",
                    "Automotivo",
                    "Educação",
                    "Tecnologia"
                };

                var criadasCount = 0;
                var categoriasIds = new Dictionary<string, int>();

                foreach (var nome in categorias)
                {
                    var result = await _categoriaService.CreateAsync(new CreateCategoriaRequest
                    {
                        Nome = nome
                    });

                    if (result.Success && result.Data != null)
                    {
                        criadasCount++;
                        categoriasIds[nome] = result.Data.CategoriaId;
                        _logger.LogInformation("Categoria criada: {Nome}", nome);
                    }
                    else
                    {
                        _logger.LogWarning("Falha ao criar categoria: {Nome}", nome);
                    }
                }

                return Ok(new 
                { 
                    success = true, 
                    message = $"{criadasCount} categorias criadas com sucesso.",
                    categorias = categoriasIds.Select(c => new { nome = c.Key, id = c.Value })
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar categorias seed");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Cria subcategorias base do sistema (protegido em produção)
        /// </summary>
        [HttpPost("seed-subcategorias")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SeedSubCategorias()
        {
            try
            {
                // Buscar todas as categorias
                var categoriasResult = await _categoriaService.GetAllAsync();
                if (!categoriasResult.Success || categoriasResult.Data?.Any() != true)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "Nenhuma categoria encontrada. Execute /seed-categorias primeiro."
                    });
                }

                var categorias = categoriasResult.Data.ToDictionary(c => c.Nome, c => c.CategoriaId);

                // Verificar se já existem subcategorias
                var existingSubCategories = await _subCategoriaService.GetAllAsync();
                if (existingSubCategories.Success && existingSubCategories.Data?.Any() == true)
                {
                    return BadRequest(new 
                    { 
                        success = false, 
                        message = "SubCategorias já existem no sistema.",
                        count = existingSubCategories.Data.Count()
                    });
                }

                // SubCategorias por categoria
                var subCategorias = new Dictionary<string, List<string>>
                {
                    ["Beleza e Estética"] = new()
                    {
                        "Barbeiro",
                        "Manicure e Pedicure",
                        "Estetica"
                    },
                    ["Saúde e Bem-Estar"] = new()
                    {
                        "Personal Trainer",
                        "Nutrição",
                        "Psicologia"
                    },
                    ["Casa e Manutenção"] = new()
                    {
                        "Serviços Gerais",
                        "Jardinagem",
                        "Limpeza Residencial"
                    },
                    ["Automotivo"] = new()
                    {
                        "Mecânica Geral",
                        "Estética/Limpeza Automotiva",
                        "Funilaria e Pintura"
                    },
                    ["Educação"] = new()
                    {
                        "Idiomas",
                        "Alfabetização",
                        "Reforço Escolar"
                    },
                    ["Tecnologia"] = new()
                    {
                        "Suporte Técnico",
                        "Desenvolvimento de Sites",
                        "Recuperação de Dados"
                    }
                };

                var criadasCount = 0;
                var errosCount = 0;

                foreach (var (categoriaNome, subs) in subCategorias)
                {
                    if (!categorias.TryGetValue(categoriaNome, out var categoriaId))
                    {
                        _logger.LogWarning("Categoria não encontrada: {Nome}", categoriaNome);
                        continue;
                    }

                    foreach (var nome in subs)
                    {
                        try
                        {
                            var result = await _subCategoriaService.CreateAsync(new CreateSubCategoriaRequest
                            {
                                Nome = nome,
                                CategoriaId = categoriaId
                            });

                            if (result.Success)
                            {
                                criadasCount++;
                                _logger.LogInformation("SubCategoria criada: {Nome} -> {Categoria}", nome, categoriaNome);
                            }
                            else
                            {
                                errosCount++;
                                _logger.LogWarning("Falha ao criar subcategoria: {Nome}", nome);
                            }
                        }
                        catch (Exception ex)
                        {
                            errosCount++;
                            _logger.LogError(ex, "Erro ao criar subcategoria: {Nome}", nome);
                        }
                    }
                }

                return Ok(new 
                { 
                    success = true, 
                    message = $"{criadasCount} subcategorias criadas com sucesso. {errosCount} erros.",
                    criadas = criadasCount,
                    erros = errosCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar subcategorias seed");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Executa seed completo: categorias + subcategorias (protegido em produção)
        /// </summary>
        [HttpPost("seed-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SeedAll()
        {
            try
            {
                var results = new List<string>();

                // 1. Seed Categorias
                var categoriasResult = await SeedCategorias();
                if (categoriasResult is OkObjectResult okCategorias)
                {
                    results.Add("Categorias criadas");
                }
                else if (categoriasResult is BadRequestObjectResult badCategorias)
                {
                    results.Add("Categorias: já existem");
                }

                // 2. Seed SubCategorias
                var subCategoriasResult = await SeedSubCategorias();
                if (subCategoriasResult is OkObjectResult okSubs)
                {
                    results.Add("SubCategorias criadas");
                }
                else if (subCategoriasResult is BadRequestObjectResult badSubs)
                {
                    results.Add("SubCategorias: já existem");
                }

                return Ok(new 
                { 
                    success = true, 
                    message = "Seed completo executado",
                    results = results
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar seed completo");
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
