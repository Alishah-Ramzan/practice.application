using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Repo.Context;
using Repo.Interfaces;
using Repo.Models;
using Repo.Repositories;
using Serilog;

class Program
{
    static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Log.Information("Application Starting");

        try
        {
            var services = new ServiceCollection();

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddHttpClient<IPixabayService, PixabayService>();
            services.AddScoped<IDumpingService, DumpingService>();

            var serviceProvider = services.BuildServiceProvider();

            var repo = serviceProvider.GetRequiredService<IProductRepository>();
            var pixabayService = serviceProvider.GetRequiredService<IPixabayService>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var dumpingService = serviceProvider.GetRequiredService<IDumpingService>();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n--- Product Management ---");
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. List All Products");
                Console.WriteLine("3. Get Product by ID");
                Console.WriteLine("4. Update Product");
                Console.WriteLine("5. Delete Product");
                Console.WriteLine("6. Exit");
                Console.WriteLine("7. Search Pixabay Images");
                Console.Write("Select an option (auto-dump in 10 sec): ");

                string input = await ReadLineWithTimeoutAsync(TimeSpan.FromSeconds(10));
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("\n⏱️ No input in 10 seconds. Dumping last records and exiting...");

                    // Dump from Pixabay service
                    await dumpingService.DumpDataAsync();

                    // Dump last product
                    var lastProduct = await dbContext.Products
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefaultAsync();

                    if (lastProduct != null)
                    {
                        var productJson = JsonSerializer.Serialize(lastProduct, new JsonSerializerOptions { WriteIndented = true });
                        await File.WriteAllTextAsync("product-dump.json", productJson);
                        Console.WriteLine("✅ Last product dumped to 'product-dump.json'");
                    }
                    else
                    {
                        Console.WriteLine("⚠️ No products found to dump.");
                    }

                    return;
                }

                switch (input)
                {
                    case "1":
                        await AddProductAsync(repo);
                        break;
                    case "2":
                        await ListAllProductsAsync(repo);
                        break;
                    case "3":
                        await GetProductByIdAsync(repo);
                        break;
                    case "4":
                        await UpdateProductAsync(repo);
                        break;
                    case "5":
                        await DeleteProductAsync(repo);
                        break;
                    case "6":
                        exit = true;
                        Console.WriteLine("Exiting...");
                        break;
                    case "7":
                        await SearchPixabayImagesAsync(pixabayService, dbContext);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static async Task<string> ReadLineWithTimeoutAsync(TimeSpan timeout)
    {
        var task = Task.Run(() => Console.ReadLine());
        if (await Task.WhenAny(task, Task.Delay(timeout)) == task)
            return task.Result;
        return null;
    }

    private static async Task AddProductAsync(IProductRepository repo)
    {
        Console.Write("Enter product name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Enter product price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("Invalid price.");
            return;
        }

        var product = new Product { Name = name, Price = price };
        await repo.AddProduct(product);
        Console.WriteLine($"Product added with ID: {product.Id}");
    }

    private static async Task ListAllProductsAsync(IProductRepository repo)
    {
        var products = await repo.GetAllProducts();
        Console.WriteLine("\nProduct List:");
        foreach (var p in products)
        {
            Console.WriteLine($"ID: {p.Id}, Name: {p.Name}, Price: {p.Price:C}");
        }
    }

    private static async Task GetProductByIdAsync(IProductRepository repo)
    {
        Console.Write("Enter product ID: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var product = await repo.GetProductById(id);
        if (product == null)
            Console.WriteLine("Product not found.");
        else
            Console.WriteLine($"ID: {product.Id}, Name: {product.Name}, Price: {product.Price:C}");
    }

    private static async Task UpdateProductAsync(IProductRepository repo)
    {
        Console.Write("Enter product ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var product = await repo.GetProductById(id);
        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        Console.Write($"Enter new name (leave blank to keep '{product.Name}'): ");
        string name = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(name))
            product.Name = name;

        Console.Write($"Enter new price (leave blank to keep {product.Price:C}): ");
        string priceInput = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(priceInput))
        {
            if (decimal.TryParse(priceInput, out decimal price))
                product.Price = price;
            else
            {
                Console.WriteLine("Invalid price input. Update canceled.");
                return;
            }
        }

        await repo.UpdateProduct(product);
        Console.WriteLine("Product updated.");
    }

    private static async Task DeleteProductAsync(IProductRepository repo)
    {
        Console.Write("Enter product ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        var product = await repo.GetProductById(id);
        if (product == null)
        {
            Console.WriteLine("Product not found.");
            return;
        }

        await repo.DeleteProduct(id);
        Console.WriteLine("Product deleted.");
    }

    private static async Task SearchPixabayImagesAsync(IPixabayService pixabayService, ApplicationDbContext dbContext)
    {
        Console.Write("Enter search keyword for images: ");
        string query = Console.ReadLine() ?? "";

        var pixabayResponse = await pixabayService.SearchImagesAsync(query);

        if (pixabayResponse?.Hits?.Length > 0)
        {
            var firstHit = pixabayResponse.Hits.First();

            var image = new Image
            {
                Query = query,
                Tag = firstHit.Tags,
                Url = firstHit.WebformatURL
            };

            dbContext.Images.Add(image);
            await dbContext.SaveChangesAsync();

            Console.WriteLine("Image saved to database:");
            Console.WriteLine($"Tags: {firstHit.Tags}");
            Console.WriteLine($"Image URL: {firstHit.WebformatURL}");
        }
        else
        {
            Console.WriteLine("No images found.");
        }
    }
}
