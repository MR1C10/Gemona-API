namespace Gemona.Application.DTOs.Request.Servico
{
    public class BuscarServicosRequest
    {
        public string? Termo { get; set; }
        public int? CategoriaId { get; set; }
        public int? SubCategoriaId { get; set; }
        public decimal? PrecoMinimo { get; set; }
        public decimal? PrecoMaximo { get; set; }
        public string? Cidade { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public double? RaioKm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}