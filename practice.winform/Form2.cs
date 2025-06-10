using AutoMapper;
using DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repo.Context;
using Repo.Mappings;

namespace practice.winform
{
    public partial class Form2 : Form
    {
        private readonly string _connectionString;
        private readonly IMapper _mapper;

        public Form2()
        {
            InitializeComponent();
            _connectionString = LoadConnectionString();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = config.CreateMapper();
        }

        private string LoadConnectionString()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json.");

            return connectionString;
        }

        private DbContextOptions<ApplicationDbContext> GetDbOptions()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;
        }

        private async void Submit_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Product name is required.");
                return;
            }

            if (!decimal.TryParse(textBox2.Text.Trim(), out decimal price))
            {
                MessageBox.Show("Please enter a valid price.");
                return;
            }

            var productDto = new ProductDto
            {
                Name = name,
                Price = price
            };

            using var context = new ApplicationDbContext(GetDbOptions());
            var repo = new ProductRepository(context, _mapper); // ✅ FIXED: passing _mapper

            try
            {
                await repo.AddProduct(productDto); // ✅ Use DTO only
                MessageBox.Show("Product added successfully!");
                textBox1.Clear();
                textBox2.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void view_Click(object sender, EventArgs e)
        {
            using var context = new ApplicationDbContext(GetDbOptions());
            var repo = new ProductRepository(context, _mapper); // ✅ FIXED: passing _mapper

            try
            {
                var productDtos = await repo.GetAllProducts(); // ✅ Already returns DTOs

                dataGridView1.DataSource = productDtos;

                if (dataGridView1.Columns["Name"] != null)
                    dataGridView1.Columns["Name"].HeaderText = "Product Name";
                if (dataGridView1.Columns["Price"] != null)
                    dataGridView1.Columns["Price"].HeaderText = "Price";

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}");
            }
        }

        private void Form2_Load(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }
}
