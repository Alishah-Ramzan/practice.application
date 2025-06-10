// IProductRepository.cs
using DTOs;

public interface IProductRepository
{
    Task AddProduct(ProductDto productDto);
    Task<List<ProductDto>> GetAllProducts();
    Task<ProductDto> GetProductById(int id);
    Task UpdateProduct(int id, ProductDto productDto);
    Task DeleteProduct(int id);
}
