namespace Gemona.Application.DTOs.Shared
{
    public abstract class BaseResponse
    {
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public bool Ativo { get; set; }
    }
}