using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Repositories;
using Gemona.Application.Exceptions;
using Gemona.Domain.Entities;
using Gemona.Domain.ValueObjects;

namespace Gemona.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ILogger<TestController> _logger;

        public TestController(
            IClienteRepository clienteRepository, 
            ICategoriaRepository categoriaRepository,
            ILogger<TestController> logger)
        {
            _clienteRepository = clienteRepository;
            _categoriaRepository = categoriaRepository;
            _logger = logger;
        }

        [HttpGet("conexao")]
        public async Task<IActionResult> TestarConexao()
        {
            try
            {
                var countClientes = await _clienteRepository.CountAsync();
                var countCategorias = await _categoriaRepository.CountAsync();

                return Ok(new
                {
                    Status = "✅ Sucesso",
                    Mensagem = "Injeção de dependências funcionando!",
                    Dados = new
                    {
                        TotalClientes = countClientes,
                        TotalCategorias = countCategorias
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Status = "❌ Erro",
                    Mensagem = ex.Message
                });
            }
        }

        [HttpPost("categoria")]
        public async Task<IActionResult> CriarCategoriaTest()
        {
            try
            {
                var categoria = new Categoria
                {
                    Nome = "Categoria Teste",
                    ImagemCategoriaUrl = "https://petitgato.com.br/img/webp/gatos-persas-em-sao-paulo-img-3780.webp"
                };

                await _categoriaRepository.AddAsync(categoria);
                await _categoriaRepository.SaveChangesAsync();

                return Ok(new
                {
                    Status = "✅ Sucesso",
                    Mensagem = "Categoria criada com sucesso!",
                    Categoria = new
                    {
                        categoria.CategoriaId,
                        categoria.Nome,
                        categoria.DataCriacao,
                        categoria.Ativo
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Status = "❌ Erro",
                    Mensagem = ex.Message
                });
            }
        }

        [HttpGet("exception/notfound")]
        public IActionResult TestNotFoundException()
        {
            _logger.LogWarning("Testando NotFoundException - Cliente ID 999 não encontrado");
            throw new NotFoundException("Cliente", 999);
        }

        [HttpGet("exception/business")]
        public IActionResult TestBusinessException()
        {
            _logger.LogWarning("Testando BusinessException - Operação não permitida");
            throw new BusinessException("Não é possível realizar esta operação no momento.");
        }

        [HttpGet("exception/unauthorized")]
        public IActionResult TestUnauthorizedException()
        {
            _logger.LogWarning("Testando UnauthorizedException - Acesso não autorizado");
            throw new UnauthorizedException();
        }

        [HttpGet("exception/generic")]
        public IActionResult TestGenericException()
        {
            _logger.LogError("Testando exceção genérica");
            throw new InvalidOperationException("Esta é uma exceção genérica para teste.");
        }

        [HttpGet("exception/argument")]
        public IActionResult TestArgumentException()
        {
            _logger.LogWarning("Testando ArgumentException - Argumento inválido");
            throw new ArgumentException("O argumento fornecido é inválido.");
        }

        [HttpGet("logs/teste")]
        public IActionResult TestLogs()
        {
            _logger.LogTrace("Este é um log TRACE");
            _logger.LogDebug("Este é um log DEBUG com informação: {Info}", "dados de debug");
            _logger.LogInformation("Este é um log INFORMATION - Operação bem-sucedida");
            _logger.LogWarning("Este é um log WARNING - Algo pode estar errado");
            _logger.LogError("Este é um log ERROR - Erro ocorreu");
            _logger.LogCritical("Este é um log CRITICAL - Sistema em risco!");

            return Ok(new
            {
                Message = "Logs de teste criados! Verifique o console e o arquivo logs/gemona-*.log",
                Levels = new[] { "Trace", "Debug", "Information", "Warning", "Error", "Critical" }
            });
        }
    }
}