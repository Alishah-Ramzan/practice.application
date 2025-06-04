using Microsoft.Extensions.Logging;
using Repo.Context;
using Repo.Interfaces;
using Repo.Models;
using System;
using System.Linq;
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

                    _logger.LogInformation("Cloned and inserted last product (ID {OriginalId}) as new product (ID {NewId})",
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
    }
}
