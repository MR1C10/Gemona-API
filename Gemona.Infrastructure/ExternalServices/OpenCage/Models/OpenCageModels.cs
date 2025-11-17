using System.Text.Json.Serialization;

namespace Gemona.Infrastructure.ExternalServices.OpenCage.Models
{
    public class OpenCageResponse
    {
        [JsonPropertyName("results")]
        public List<OpenCageResult> Results { get; set; } = new();

        [JsonPropertyName("status")]
        public OpenCageStatus Status { get; set; } = new();
    }

    public class OpenCageResult
    {
        [JsonPropertyName("formatted")]
        public string Formatted { get; set; } = string.Empty;

        [JsonPropertyName("geometry")]
        public OpenCageGeometry Geometry { get; set; } = new();

        [JsonPropertyName("components")]
        public OpenCageComponents Components { get; set; } = new();
    }

    public class OpenCageGeometry
    {
        [JsonPropertyName("lat")]
        public decimal Lat { get; set; }

        [JsonPropertyName("lng")]
        public decimal Lng { get; set; }
    }

    public class OpenCageComponents
    {
        [JsonPropertyName("road")]
        public string? Road { get; set; }

        [JsonPropertyName("house_number")]
        public string? HouseNumber { get; set; }

        [JsonPropertyName("neighbourhood")]
        public string? Neighbourhood { get; set; }

        [JsonPropertyName("suburb")]
        public string? Suburb { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("postcode")]
        public string? Postcode { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }
    }

    public class OpenCageStatus
    {
        [JsonPropertyName("code")]
        public int Code { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
