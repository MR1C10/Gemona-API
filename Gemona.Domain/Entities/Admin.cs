using Microsoft.AspNetCore.Identity;

namespace Gemona.Domain.Entities
{
    public class Admin : IdentityUser<int>
    {
        public string Nome { get; set; } = string.Empty;
        public string GithubUsername { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; } = true;
    }
}
