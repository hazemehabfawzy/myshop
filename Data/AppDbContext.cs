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

            // Product entity configuration
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.StockStatus)
                      .HasConversion<string>()
                      .HasDefaultValue(StockStatus.InStock);
            });



            // Order - OrderItem (One-to-Many)
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);
                entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(o => o.ShippingAddress).IsRequired();
                entity.Property(o => o.PaymentMethod).IsRequired();
                entity.ToTable(t => t.HasCheckConstraint("CK_Order_PaymentMethod", "PaymentMethod = 'CashOnDelivery'"));
                entity.HasOne(o => o.User)
                      .WithMany(u => u.Orders)
                      .HasForeignKey(o => o.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(o => o.OrderItems)
                      .WithOne(oi => oi.Order)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // OrderItem
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);
                entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
                entity.HasOne(oi => oi.Product)
                      .WithMany(p => p.OrderItems)
                      .HasForeignKey(oi => oi.ProductId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ServiceRequest
            modelBuilder.Entity<ServiceRequest>(entity =>
            {
                entity.HasKey(sr => sr.Id);
                entity.Property(sr => sr.EstimatedCost).HasColumnType("decimal(18,2)");
                entity.Property(sr => sr.FinalCost).HasColumnType("decimal(18,2)");
                entity.HasOne(sr => sr.User)
                      .WithMany(u => u.CustomerServiceRequests)
                      .HasForeignKey(sr => sr.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

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
