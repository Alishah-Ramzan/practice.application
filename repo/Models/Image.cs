using System;
using System.Text.Json.Serialization;

namespace Repo.Models
{
    public class Image
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("query")]
        public string Query { get; set; } = "";

        [JsonPropertyName("tag")]
        public string Tag { get; set; } = "";

        [JsonPropertyName("url")]
        public string Url { get; set; } = "";
    }
}
