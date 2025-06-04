using System.Text.Json.Serialization;

namespace Repo.Models
{
    public class PixabayResponse
    {
        [JsonPropertyName("hits")]
        public PixabayHit[] Hits { get; set; } = Array.Empty<PixabayHit>();
    }

    public class PixabayHit
    {
        [JsonPropertyName("tags")]
        public string Tags { get; set; } = "";

        [JsonPropertyName("webformatURL")]
        public string WebformatURL { get; set; } = "";
    }
}
