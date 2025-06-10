using Repo.Context;
using Repo.Repositories;
using Repo.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Repo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DTOs;

namespace practice.winform
{
    public partial class Form1 : Form
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly ProductRepository _repo;
        private List<PixabayHit> _pixabayHits = new();

        public Form1()
        {
            InitializeComponent();

            _connectionString = LoadConnectionString();
            _mapper = ConfigureMapper();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            _context = new ApplicationDbContext(options);
            _repo = new ProductRepository(_context, _mapper);
        }

        private string LoadConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var connectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' is not found or is null.");
            }

            return connectionString;
        }

        private IMapper ConfigureMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            return config.CreateMapper();
        }

        private async void button1_Click(object sender, EventArgs e)  // Add Product
        {
            string name = textBox1.Text.Trim();
            if (!decimal.TryParse(textBox2.Text.Trim(), out decimal price))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }

            var productDto = new ProductDto { Name = name, Price = price };

            try
            {
                await _repo.AddProduct(productDto);
                MessageBox.Show("Product added successfully!");
                textBox1.Clear();
                textBox2.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void button1_Click_1(object sender, EventArgs e)  // Load all products
        {
            try
            {
                var products = await _repo.GetAllProducts();
                dataGridView1.DataSource = products;

                if (dataGridView1.Columns["Name"] != null)
                    dataGridView1.Columns["Name"].HeaderText = "Product Name";
                if (dataGridView1.Columns["Price"] != null)
                    dataGridView1.Columns["Price"].HeaderText = "Price";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
            }
        }

        private async void button2_Click(object sender, EventArgs e)  // Update product
        {
            if (!int.TryParse(textBox3.Text.Trim(), out int productId))
            {
                MessageBox.Show("Please enter a valid numeric ID.");
                return;
            }

            string name = textBox1.Text.Trim();
            if (!decimal.TryParse(textBox2.Text.Trim(), out decimal price))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }

            var productDto = new ProductDto { Name = name, Price = price };

            try
            {
                var existingProduct = await _repo.GetProductById(productId);
                if (existingProduct == null)
                {
                    MessageBox.Show("Product not found.");
                    return;
                }

                await _repo.UpdateProduct(productId, productDto);
                MessageBox.Show("Product updated successfully!");
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void button3_Click(object sender, EventArgs e)  // Delete product
        {
            if (!int.TryParse(textBox3.Text.Trim(), out int productId))
            {
                MessageBox.Show("Please enter a valid numeric Product ID.");
                return;
            }

            try
            {
                await _repo.DeleteProduct(productId);
                MessageBox.Show("Product deleted successfully!");
                textBox3.Clear();
                textBox1.Clear();
                textBox2.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // The Pixabay image search functionality is left untouched except context usage
        private async void button4_Click(object sender, EventArgs e)
        {
            string query = textBox4.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Please enter a search keyword.");
                return;
            }

            var pixabayService = new PixabayService(new System.Net.Http.HttpClient(), _context);

            try
            {
                var pixabayResponse = await pixabayService.SearchImagesAsync(query);

                if (pixabayResponse?.Hits?.Length > 0)
                {
                    _pixabayHits = pixabayResponse.Hits.ToList();

                    dataGridView2.DataSource = _pixabayHits.Select(hit => new
                    {
                        Tags = hit.Tags,
                        ImageURL = hit.WebformatURL
                    }).ToList();

                    MessageBox.Show("Top search image added to database.");
                }
                else
                {
                    MessageBox.Show("No images found.");
                    dataGridView2.DataSource = null;
                    _pixabayHits.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error searching images: {ex.Message}");
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            string filter = textBox4.Text.Trim().ToLower();

            var filtered = string.IsNullOrEmpty(filter)
                ? _pixabayHits
                : _pixabayHits.Where(hit => hit.Tags.ToLower().Contains(filter));

            dataGridView2.DataSource = filtered.Select(hit => new
            {
                Tags = hit.Tags,
                ImageURL = hit.WebformatURL
            }).ToList();
        }

        // Other event handlers (empty)
        private void Form1_Load(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e) { }

        private void button5_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide(); 
        }
    }
}
