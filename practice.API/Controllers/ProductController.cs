using DTOs;
using Microsoft.AspNetCore.Mvc;
using Repo.Interfaces;

namespace practice.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productRepository.GetAllProducts();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult> Create(ProductDto productDto)
        {
            await _productRepository.AddProduct(productDto);
            return CreatedAtAction(nameof(GetById), new { id = 0 }, productDto); // Replace '0' with the actual ID if available
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, ProductDto productDto)
        {
            var productExists = await _productRepository.GetProductById(id) != null;
            if (!productExists)
                return NotFound();

            await _productRepository.UpdateProduct(id, productDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var product = await _productRepository.GetProductById(id);
            if (product == null)
                return NotFound();

            await _productRepository.DeleteProduct(id);
            return NoContent();
        }
    }
}
