using Gemona.Domain.ValueObjects;

namespace Gemona.Domain.Entities
{
    public class Estabelecimento : BaseEntity
    {
        public int EstabelecimentoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public Cnpj Cnpj { get; set; } = null!;
        public string? ImagemEstabelecimentoUrl { get; set; }
        public int ProfissionalId { get; set; }
        public int EnderecoId { get; set; }

        public virtual Profissional Profissional { get; set; } = null!;
        public virtual Endereco Endereco { get; set; } = null!;
        public virtual ICollection<HorarioFuncionamento> HorarioFuncionamento { get; set; } = new List<HorarioFuncionamento>();
        public virtual ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    }
}