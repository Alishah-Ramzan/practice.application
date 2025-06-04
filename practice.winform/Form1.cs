using Repo.Context;
using Repo.Models;
using Repo.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace practice.winform
{
    public partial class Form1 : Form
    {
        private readonly string _connectionString;
        private List<PixabayHit> _pixabayHits = new();

        public Form1()
        {
            InitializeComponent();
            _connectionString = LoadConnectionString();
        }

        private string LoadConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            return config.GetConnectionString("DefaultConnection");
        }

        // Add Product
        private async void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();

            if (!decimal.TryParse(textBox2.Text.Trim(), out decimal price))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }

            var product = new Product
            {
                Name = name,
                Price = price
            };

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var context = new ApplicationDbContext(options);
            var repo = new ProductRepository(context);

            try
            {
                await repo.AddProduct(product);
                MessageBox.Show("Product added successfully!");
                textBox1.Clear();
                textBox2.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // Load all products
        private async void button1_Click_1(object sender, EventArgs e)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var context = new ApplicationDbContext(options);
            var repo = new ProductRepository(context);

            try
            {
                var products = await repo.GetAllProducts();
                dataGridView1.DataSource = products;

                if (dataGridView1.Columns["Id"] != null)
                    dataGridView1.Columns["Id"].HeaderText = "Product ID";
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

        // Update product
        private async void button2_Click(object sender, EventArgs e)
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

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var context = new ApplicationDbContext(options);
            var repo = new ProductRepository(context);

            try
            {
                var existingProduct = await repo.GetProductById(productId);
                if (existingProduct == null)
                {
                    MessageBox.Show("Product not found.");
                    return;
                }

                existingProduct.Name = name;
                existingProduct.Price = price;

                await repo.UpdateProduct(existingProduct);
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

        // Delete product
        private async void button3_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox3.Text.Trim(), out int productId))
            {
                MessageBox.Show("Please enter a valid numeric Product ID.");
                return;
            }

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var context = new ApplicationDbContext(options);
            var repo = new ProductRepository(context);

            try
            {
                var product = await repo.GetProductById(productId);
                if (product == null)
                {
                    MessageBox.Show("Product not found.");
                    return;
                }

                await repo.DeleteProduct(productId);
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

        // Search Pixabay Images button click
        private async void button4_Click(object sender, EventArgs e)
        {
            string query = textBox4.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Please enter a search keyword.");
                return;
            }

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            using var context = new ApplicationDbContext(options);
            var pixabayService = new PixabayService(new System.Net.Http.HttpClient(), context);

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

        // Filter the Pixabay images displayed in dataGridView2 by Tags as user types in textBox4
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            string filter = textBox4.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(filter))
            {
                dataGridView2.DataSource = _pixabayHits.Select(hit => new
                {
                    Tags = hit.Tags,
                    ImageURL = hit.WebformatURL
                }).ToList();
            }
            else
            {
                var filtered = _pixabayHits
                    .Where(hit => hit.Tags.ToLower().Contains(filter))
                    .Select(hit => new
                    {
                        Tags = hit.Tags,
                        ImageURL = hit.WebformatURL
                    }).ToList();

                dataGridView2.DataSource = filtered;
            }
        }

        // Other unused event handlers you had (can keep empty)
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
    }
}
