using Gemona.Domain.ValueObjects;

namespace Gemona.Domain.Entities
{
    public class Profissional : BaseEntity
    {
        public int ProfissionalId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public Cpf? Cpf { get; set; } = null!;
        public string? ImagemPerfilUrl { get; set; }
        public DateTime DataNacimento { get; set; }
        public string SenhaHash { get; set; } = string.Empty;

        public virtual Estabelecimento? Estabelecimento { get; set; }
    }
}