using Microsoft.AspNetCore.Mvc;
using Gemona.Application.Interfaces.Repositories;
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

        public TestController(IClienteRepository clienteRepository, ICategoriaRepository categoriaRepository)
        {
            _clienteRepository = clienteRepository;
            _categoriaRepository = categoriaRepository;
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
    }
}