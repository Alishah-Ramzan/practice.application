﻿using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repo.Context;
using Repo.Interfaces;
using Repo.Repositories;
using DTOs;
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

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDumpingService, DumpingService>();

            var serviceProvider = services.BuildServiceProvider();

            var productRepo = serviceProvider.GetRequiredService<IProductRepository>();
            var userRepo = serviceProvider.GetRequiredService<IUserRepository>();
            var dumpingService = serviceProvider.GetRequiredService<IDumpingService>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n--- Management Console ---");
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. List All Products");
                Console.WriteLine("3. Get Product by ID");
                Console.WriteLine("4. Update Product");
                Console.WriteLine("5. Delete Product");
                Console.WriteLine("6. Exit (Auto-dump on timeout)");
                Console.WriteLine("7. Recover Last Dumped Product");
                Console.WriteLine("8. Exit Immediately");
                Console.WriteLine("9. Add User");
                Console.WriteLine("10. List All Users");
                Console.Write("Select an option (auto-dump in 10 sec): ");

                string input = await ReadLineWithTimeoutAsync(TimeSpan.FromSeconds(10));
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("\n⏱️ No input in 10 seconds. Dumping last records and exiting...");

                    await dumpingService.DumpDataAsync();

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
                        await AddProductAsync(productRepo);
                        break;
                    case "2":
                        await ListAllProductsAsync(productRepo);
                        break;
                    case "3":
                        await GetProductByIdAsync(productRepo);
                        break;
                    case "4":
                        await UpdateProductAsync(productRepo);
                        break;
                    case "5":
                        await DeleteProductAsync(productRepo);
                        break;
                    case "6":
                        exit = true;
                        Console.WriteLine("Exiting...");
                        break;
                    case "7":
                        await dumpingService.RecoverLastDumpedProductAsync();
                        break;
                    case "8":
                        exit = true;
                        Console.WriteLine("Immediate exit.");
                        break;
                    case "9":
                        await AddUserAsync(userRepo);
                        break;
                    case "10":
                        await ListAllUsersAsync(userRepo);
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
            return task.Result ?? string.Empty;
        return string.Empty;
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

        var dto = new ProductDto { Name = name, Price = price };
        await repo.AddProduct(dto);
        Console.WriteLine("✅ Product added.");
    }

    private static async Task ListAllProductsAsync(IProductRepository repo)
    {
        var products = await repo.GetAllProducts();
        Console.WriteLine("\n📦 Product List:");
        foreach (var p in products)
        {
            Console.WriteLine($"Name: {p.Name}, Price: {p.Price:C}");
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
            Console.WriteLine("❌ Product not found.");
        else
            Console.WriteLine($"Name: {product.Name}, Price: {product.Price:C}");
    }

    private static async Task UpdateProductAsync(IProductRepository repo)
    {
        Console.Write("Enter product ID to update: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        Console.Write("Enter new name: ");
        string name = Console.ReadLine() ?? "";

        Console.Write("Enter new price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("Invalid price.");
            return;
        }

        var updatedDto = new ProductDto { Name = name, Price = price };
        await repo.UpdateProduct(id, updatedDto);
        Console.WriteLine("✅ Product updated.");
    }

    private static async Task DeleteProductAsync(IProductRepository repo)
    {
        Console.Write("Enter product ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Invalid ID.");
            return;
        }

        await repo.DeleteProduct(id);
        Console.WriteLine("🗑️ Product deleted.");
    }

    private static async Task AddUserAsync(IUserRepository userRepo)
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine() ?? "";

        Console.Write("Enter password: ");
        string password = Console.ReadLine() ?? "";

        var user = new Repo.Models.User { Username = username, Password = password };
        await userRepo.AddUserAsync(user);
        Console.WriteLine("✅ User added.");
    }

    private static async Task ListAllUsersAsync(IUserRepository userRepo)
    {
        var users = await userRepo.GetAllUsersAsync();
        Console.WriteLine("\n👤 User List:");
        foreach (var user in users)
        {
            Console.WriteLine($"ID: {user.Id}, Username: {user.Username}");
        }
    }
}
