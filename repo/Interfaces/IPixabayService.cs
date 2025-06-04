using System.Threading.Tasks;
using Repo.Models;

public interface IPixabayService
{
    Task<PixabayResponse> SearchImagesAsync(string query);
}
