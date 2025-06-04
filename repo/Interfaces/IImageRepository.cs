using Repo.Models;
using System.Threading.Tasks;

namespace Repo.Interfaces
{
    public interface IImageRepository
    {
        Task AddImageAsync(Image image);
    }
}
