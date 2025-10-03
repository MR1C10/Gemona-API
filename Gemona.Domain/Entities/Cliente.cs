using Gemona.Domain.ValueObjects;

namespace Gemona.Domain.Entities
{
    public class Cliente : BaseEntity
    {
        public int ClienteId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public Cpf Cpf { get; set; } = null!;
        public string? ImagemPerfilUrl { get; set; }
        public int? EnderecoId { get; set; }
        public DateTime DataNascimento { get; set; }
        public string SenhaHash { get; set; } = string.Empty;

        public virtual Endereco? Endereco { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
        public virtual ICollection<Avaliacao> Avaliacoes { get; set; } = new List<Avaliacao>();

    }
}