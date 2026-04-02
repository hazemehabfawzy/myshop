using Microsoft.EntityFrameworkCore;
using TechVault.API.Models.Entities;

namespace TechVault.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<CustomerProfile> CustomerProfiles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - CustomerProfile (One-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.CustomerProfile)
                .WithOne(cp => cp.User)
                .HasForeignKey<CustomerProfile>(cp => cp.UserId);

            // Category - Category (Self-referencing One-to-Many)
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Category - Product (One-to-Many)
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);

            // Product - ProductSpecification (One-to-One)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Specification)
                .WithOne(ps => ps.Product)
                .HasForeignKey<ProductSpecification>(ps => ps.ProductId);

            // User - Order (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

            // Order - OrderItem (One-to-Many)
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId);

            // Product - OrderItem (One-to-Many)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId);

            // User - ServiceRequest (Customer, One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.CustomerServiceRequests)
                .WithOne(sr => sr.User)
                .HasForeignKey(sr => sr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User - ServiceRequest (Technician, One-to-Many, Nullable)
            modelBuilder.Entity<User>()
                .HasMany(u => u.TechnicianServiceRequests)
                .WithOne(sr => sr.Technician)
                .HasForeignKey(sr => sr.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - Tag (Many-to-Many)
            modelBuilder.Entity<ProductTag>()
                .HasKey(pt => new { pt.ProductId, pt.TagId });

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTags)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<ProductTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.ProductTags)
                .HasForeignKey(pt => pt.TagId);
        }
    }
}
