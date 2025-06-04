using Repo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IProductRepository
{
    Task AddProduct(Product product);
    Task<List<Product>> GetAllProducts();
    Task<Product?> GetProductById(int id);
    Task UpdateProduct(Product product);
    Task DeleteProduct(int id);
}
