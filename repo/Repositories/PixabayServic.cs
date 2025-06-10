using Repo.Context;
using Repo.Interfaces;
using Repo.Models;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

// Alias to fix ambiguity
using ImageModel = Repo.Models.Image;

namespace Repo.Repositories
{
    public class PixabayService : IPixabayService
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _dbContext;
        private readonly string _apiKey = "44167668-06b903e4477db3fd7e90748f7";

        public PixabayService(HttpClient httpClient, ApplicationDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<PixabayResponse> SearchImagesAsync(string query)
        {
            string url = $"https://pixabay.com/api/?key={_apiKey}&q={Uri.EscapeDataString(query)}&image_type=photo&per_page=5";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var pixabayResponse = JsonSerializer.Deserialize<PixabayResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Save first image result to DB
            if (pixabayResponse?.Hits?.Length > 0)
            {
                var firstHit = pixabayResponse.Hits[0];
                var image = new ImageModel
                {
                    Query = query,
                    Tag = firstHit.Tags,
                    Url = firstHit.WebformatURL
                };

                _dbContext.Images.Add(image);
                await _dbContext.SaveChangesAsync();
            }

            return pixabayResponse ?? new PixabayResponse { Hits = Array.Empty<PixabayHit>() };
        }
    }
}
