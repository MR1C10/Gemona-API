namespace Gemona.Domain.Entities
{
    public class BaseEntity
    {
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; } = true;
    }
}