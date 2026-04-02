using Microsoft.EntityFrameworkCore;
using TechVault.API.Models.Entities;

namespace TechVault.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync()) return;

            // Seed Users
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@techvault.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin"
            };

            var techUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "tech1",
                Email = "tech1@techvault.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Tech123!"),
                Role = "Technician"
            };

            var customerUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "john",
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
                Role = "Customer"
            };

            context.Users.AddRange(adminUser, techUser, customerUser);

            // Seed Customer Profiles
            context.CustomerProfiles.Add(new CustomerProfile { Id = Guid.NewGuid(), UserId = adminUser.Id, FullName = "System Administrator" });
            context.CustomerProfiles.Add(new CustomerProfile { Id = Guid.NewGuid(), UserId = techUser.Id, FullName = "Lead Technician" });
            context.CustomerProfiles.Add(new CustomerProfile { Id = Guid.NewGuid(), UserId = customerUser.Id, FullName = "John Doe", PhoneNumber = "123456789", Address = "123 Street, City" });

            // Seed Categories
            var phonesCat = new Category { Id = Guid.NewGuid(), Name = "Phones", Description = "Smartphones and handheld devices" };
            var laptopsCat = new Category { Id = Guid.NewGuid(), Name = "Laptops", Description = "Portable computers" };
            var pcCompCat = new Category { Id = Guid.NewGuid(), Name = "PC Components", Description = "Internal computer hardware" };
            var accessoriesCat = new Category { Id = Guid.NewGuid(), Name = "Accessories", Description = "Peripherals and add-ons" };

            context.Categories.AddRange(phonesCat, laptopsCat, pcCompCat, accessoriesCat);

            var iphoneSub = new Category { Id = Guid.NewGuid(), Name = "iPhone", ParentCategoryId = phonesCat.Id };
            var samsungSub = new Category { Id = Guid.NewGuid(), Name = "Samsung", ParentCategoryId = phonesCat.Id };
            var gamingLaptopSub = new Category { Id = Guid.NewGuid(), Name = "Gaming Laptops", ParentCategoryId = laptopsCat.Id };
            var gpuSub = new Category { Id = Guid.NewGuid(), Name = "Graphics Cards", ParentCategoryId = pcCompCat.Id };
            var cpuSub = new Category { Id = Guid.NewGuid(), Name = "Processors", ParentCategoryId = pcCompCat.Id };

            context.Categories.AddRange(iphoneSub, samsungSub, gamingLaptopSub, gpuSub, cpuSub);

            // Seed Tags
            var gamingTag = new Tag { Id = Guid.NewGuid(), Name = "Gaming" };
            var budgetTag = new Tag { Id = Guid.NewGuid(), Name = "Budget" };
            var flagshipTag = new Tag { Id = Guid.NewGuid(), Name = "Flagship" };
            var proTag = new Tag { Id = Guid.NewGuid(), Name = "Professional" };
            var newArrivalTag = new Tag { Id = Guid.NewGuid(), Name = "New Arrival" };

            context.Tags.AddRange(gamingTag, budgetTag, flagshipTag, proTag, newArrivalTag);

            // Seed Products
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "iPhone 15 Pro", Brand = "Apple", Price = 999.99m, StockQuantity = 10, CategoryId = iphoneSub.Id, Description = "Latest iPhone with Titanium build", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Samsung Galaxy S24 Ultra", Brand = "Samsung", Price = 1299.99m, StockQuantity = 5, CategoryId = samsungSub.Id, Description = "Flagship Samsung with AI features", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "RTX 4090", Brand = "NVIDIA", Price = 1599.99m, StockQuantity = 2, CategoryId = gpuSub.Id, Description = "Ultimate gaming GPU", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Intel Core i9-14900K", Brand = "Intel", Price = 589.99m, StockQuantity = 8, CategoryId = cpuSub.Id, Description = "High-end desktop processor", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Razer Blade 16", Brand = "Razer", Price = 2999.99m, StockQuantity = 3, CategoryId = gamingLaptopSub.Id, Description = "Premium gaming laptop", IsActive = true }
            };

            context.Products.AddRange(products);

            // Seed Product Tags
            context.ProductTags.Add(new ProductTag { ProductId = products[0].Id, TagId = flagshipTag.Id });
            context.ProductTags.Add(new ProductTag { ProductId = products[1].Id, TagId = flagshipTag.Id });
            context.ProductTags.Add(new ProductTag { ProductId = products[2].Id, TagId = gamingTag.Id });
            context.ProductTags.Add(new ProductTag { ProductId = products[4].Id, TagId = gamingTag.Id });
            context.ProductTags.Add(new ProductTag { ProductId = products[4].Id, TagId = proTag.Id });

            await context.SaveChangesAsync();
        }
    }
}
