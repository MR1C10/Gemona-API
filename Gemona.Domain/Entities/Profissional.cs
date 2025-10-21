using Microsoft.AspNetCore.Identity;
using Gemona.Domain.ValueObjects;

namespace Gemona.Domain.Entities
{
    public class Profissional : IdentityUser<int>
    {
        public string Nome { get; set; } = string.Empty;
        public Cpf Cpf { get; set; } = null!;
        public string? ImagemPerfilUrl { get; set; }
        public DateTime DataNascimento { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; } = true;
        
        // Relacionamentos
        public virtual Estabelecimento? Estabelecimento { get; set; }
    }
}