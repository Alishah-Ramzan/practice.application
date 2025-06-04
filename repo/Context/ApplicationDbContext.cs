using Microsoft.EntityFrameworkCore;
using Repo.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

// Create an alias for your model Image to avoid ambiguity
using ImageModel = Repo.Models.Image;

namespace Repo.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;

        // Use alias here for Image DbSet
        public DbSet<ImageModel> Images { get; set; } = null!;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Fluent configuration for Product
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(p => p.Price)
                      .HasColumnType("decimal(18,2)");
            });

            // Fluent configuration for ImageModel (your Repo.Models.Image)
            modelBuilder.Entity<ImageModel>(entity =>
            {
                entity.Property(i => i.Query)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(i => i.Tag)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(i => i.Url)
                      .IsRequired()
                      .HasMaxLength(1000);
            });
        }
    }
}
