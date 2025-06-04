using Repo.Context;
using Repo.Interfaces;
using Repo.Models;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

// Add alias here:
using ImageModel = Repo.Models.Image;

namespace Repo.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Use alias to match interface method signature exactly
        public async Task AddImageAsync(ImageModel image)
        {
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
        }
    }
}
