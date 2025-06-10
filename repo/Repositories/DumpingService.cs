using Microsoft.Extensions.Logging;
using Repo.Context;
using Repo.Interfaces;
using Repo.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Repo.Repositories
{
    public class DumpingService : IDumpingService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DumpingService> _logger;

        public DumpingService(ApplicationDbContext context, ILogger<DumpingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task DumpDataAsync()
        {
            try
            {
                var lastProduct = await _context.Products
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefaultAsync();

                if (lastProduct != null)
                {
                    var clonedProduct = new Product
                    {
                        Name = lastProduct.Name + " (Dumped at " + DateTime.Now.ToString("HH:mm:ss") + ")",
                        Price = lastProduct.Price
                    };

                    _context.Products.Add(clonedProduct);
                    await _context.SaveChangesAsync();

                    var productJson = JsonSerializer.Serialize(clonedProduct, new JsonSerializerOptions { WriteIndented = true });
                    await File.WriteAllTextAsync("product-dump.json", productJson);

                    _logger.LogInformation("Cloned and inserted last product (ID {OriginalId}) as new product (ID {NewId}) and dumped to file.",
                        lastProduct.Id, clonedProduct.Id);
                }
                else
                {
                    _logger.LogWarning("No products found to dump.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while cloning and dumping product.");
            }
        }

        public async Task RecoverLastDumpedProductAsync()
        {
            try
            {
                if (!File.Exists("product-dump.json"))
                {
                    _logger.LogWarning("No dump file found for recovery.");
                    return;
                }

                var json = await File.ReadAllTextAsync("product-dump.json");
                var product = JsonSerializer.Deserialize<Product>(json);

                if (product != null)
                {
                    product.Id = 0; // Reset ID for EF Core

                    // Remove " (Dumped at ...)" if present in the name
                    var name = product.Name;
                    var index = name.IndexOf(" (Dumped at");
                    if (index != -1)
                    {
                        product.Name = name.Substring(0, index);
                    }

                    await _context.Products.AddAsync(product);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Recovered dumped product and inserted it into the database.");
                }
                else
                {
                    _logger.LogWarning("Failed to deserialize dumped product.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during product recovery.");
            }
        }

    }
}
