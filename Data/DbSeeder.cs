using Microsoft.EntityFrameworkCore;
using TechVault.API.Models.Entities;

namespace TechVault.API.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Fix 3: Clear old users and products to ensure clean re-seed
            if (await context.Users.AnyAsync())
            {
                // Clear dependent tables first to avoid foreign key constraints
                context.OrderItems.RemoveRange(context.OrderItems);
                context.Orders.RemoveRange(context.Orders);
                context.ServiceRequests.RemoveRange(context.ServiceRequests);
                context.CustomerProfiles.RemoveRange(context.CustomerProfiles);
                context.Users.RemoveRange(context.Users);
                context.Products.RemoveRange(context.Products);
                context.Categories.RemoveRange(context.Categories);
                context.Tags.RemoveRange(context.Tags);
                await context.SaveChangesAsync();
            }

            // Re-seed with correct BCrypt hashes as per Fix 3
            var defaultHash = BCrypt.Net.BCrypt.HashPassword("123456");

            var adminUser = new User { Id = Guid.NewGuid(), Username = "admin", Email = "admin@techvault.com", PasswordHash = defaultHash, Role = "Admin" };
            var techUser = new User { Id = Guid.NewGuid(), Username = "tech", Email = "tech@techvault.com", PasswordHash = defaultHash, Role = "Technician" };
            var hazemUser = new User { Id = Guid.NewGuid(), Username = "hazem", Email = "hazem@techvault.com", PasswordHash = defaultHash, Role = "Customer" };

            context.Users.AddRange(adminUser, techUser, hazemUser);
            
            context.CustomerProfiles.AddRange(
                new CustomerProfile { Id = Guid.NewGuid(), UserId = adminUser.Id, FullName = "System Admin" },
                new CustomerProfile { Id = Guid.NewGuid(), UserId = techUser.Id, FullName = "Tech Lead" },
                new CustomerProfile { Id = Guid.NewGuid(), UserId = hazemUser.Id, FullName = "Hazem Fawzy", PhoneNumber = "555-0199", Address = "Tech City, 42" }
            );

            // FIX 4 - BACKEND + SEED: Replace ALL products with PC hardware focus
            var catGPU     = new Category { Id = Guid.NewGuid(), Name = "Graphics Cards",   Description = "Dedicated GPUs for gaming and workstation" };
            var catCPU     = new Category { Id = Guid.NewGuid(), Name = "Processors",       Description = "Desktop CPUs from Intel and AMD" };
            var catRAM     = new Category { Id = Guid.NewGuid(), Name = "Memory (RAM)",     Description = "DDR4 and DDR5 memory kits" };
            var catMobo    = new Category { Id = Guid.NewGuid(), Name = "Motherboards",     Description = "ATX, mATX and ITX motherboards" };
            var catPSU     = new Category { Id = Guid.NewGuid(), Name = "Power Supplies",   Description = "80+ rated modular power supplies" };
            var catCase    = new Category { Id = Guid.NewGuid(), Name = "PC Cases",         Description = "Mid and full tower PC cases" };
            var catCooler  = new Category { Id = Guid.NewGuid(), Name = "CPU Coolers",      Description = "Air and AIO liquid coolers" };
            var catStorage = new Category { Id = Guid.NewGuid(), Name = "Storage",          Description = "NVMe SSDs and SATA drives" };
            var catMon     = new Category { Id = Guid.NewGuid(), Name = "Monitors",         Description = "Gaming and professional monitors" };
            var catPeri    = new Category { Id = Guid.NewGuid(), Name = "Peripherals",      Description = "Keyboards, mice and headsets" };
            var catPhone   = new Category { Id = Guid.NewGuid(), Name = "Phones",           Description = "Flagship smartphones" };
            var catLaptop  = new Category { Id = Guid.NewGuid(), Name = "Gaming Laptops",   Description = "High performance gaming laptops" };

            context.Categories.AddRange(catGPU, catCPU, catRAM, catMobo, catPSU, catCase, catCooler, catStorage, catMon, catPeri, catPhone, catLaptop);

            context.Products.AddRange(
                new Product { Id = Guid.NewGuid(), Name = "ASUS ROG STRIX RTX 4090 OC 24GB", Brand = "ASUS", Price = 54999, StockQuantity = 4, StockStatus = StockStatus.LowStock, CategoryId = catGPU.Id, ImageUrl = "https://dlcdnwebimgs.asus.com/gain/A5C97B8A-D93F-4B36-9A0A-FBD7BD19D6F6/w717/h525", Description = "The ultimate GeForce GPU, bringing a quantum leap in performance, efficiency, and AI-powered graphics. Experience ultra-high performance gaming, incredibly detailed virtual worlds with ray tracing, and unprecedented productivity.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "MSI RTX 4080 Super 16G Gaming X Slim", Brand = "MSI", Price = 34999, StockQuantity = 7, StockStatus = StockStatus.LowStock, CategoryId = catGPU.Id, ImageUrl = "https://asset.msi.com/resize/image/global/product/product_17001399478f2f36da3ab19eb29fefc1b63eafc3fa.png62405b38c58fe0f07fcef2367d8a9ba1/1024.png", Description = "A powerhouse GPU featuring a sleek, slimmed-down design without compromising on performance. Engineered with advanced cooling and high-speed memory to deliver fluid ray-traced graphics and stellar framerates.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Sapphire NITRO+ RX 7900 XTX 24GB", Brand = "Sapphire", Price = 28999, StockQuantity = 0, StockStatus = StockStatus.OutOfStock, CategoryId = catGPU.Id, ImageUrl = "https://www.sapphiretech.com/productimg/?image=aHR0cHM6Ly93d3cuc2FwcGhpcmV0ZWNoLmNvbS9tZWRpYWxpYi9wcm9kdWN0L3ByZW1pdW0vMTMzMDIvbWFpbl8xLmpwZw==", Description = "AMD's premier RDNA 3 graphics card, featuring massive 24GB VRAM and a striking vapor-chamber design. Delivers exceptional 4K gaming performance and incredible thermal efficiency under extreme workloads.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "AMD Ryzen 9 7950X3D", Brand = "AMD", Price = 16999, StockQuantity = 9, StockStatus = StockStatus.LowStock, CategoryId = catCPU.Id, ImageUrl = "https://www.amd.com/content/dam/amd/en/images/products/processors/ryzen/2179260-ryzen-9-7950x3d-PIB-1260x709.png", Description = "AMD's flagship desktop processor featuring 3D V-Cache technology for unmatched gaming and multithreading performance.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "AMD Ryzen 7 7800X3D", Brand = "AMD", Price = 9499, StockQuantity = 22, StockStatus = StockStatus.InStock, CategoryId = catCPU.Id, ImageUrl = "https://www.amd.com/content/dam/amd/en/images/products/processors/ryzen/2179262-ryzen-7-7800x3d-PIB-1260x709.png", Description = "Widely regarded as the absolute best gaming processor on the market, combining AMD's advanced Zen 4 architecture with massive 3D V-Cache for legendary in-game performance.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Intel Core i9-14900K", Brand = "Intel", Price = 14999, StockQuantity = 12, StockStatus = StockStatus.InStock, CategoryId = catCPU.Id, ImageUrl = "https://www.intel.com/content/dam/www/central-libraries/us/en/images/2022-11/processors-core-i9-13900k-front-angle-rwd.png.rendition.intel.web.550.309.png", Description = "A powerhouse processor featuring 24 cores and jaw-dropping boost clocks of up to 6.0GHz. Built to satisfy enthusiasts, streamers, and creative professionals demanding top-tier multithreaded power.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Corsair Dominator Titanium DDR5-6000 32GB", Brand = "Corsair", Price = 5999, StockQuantity = 25, StockStatus = StockStatus.InStock, CategoryId = catRAM.Id, ImageUrl = "https://www.corsair.com/medias/sys_master/images/images/hf8/h39/10526826471454/CMP32GX5M2B6000C30-Gallery-Dominator-Titanium-RGB-DDR5-6000MHZ-32GB-2X16GB-Black-01-CMP32GX5M2B6000C30.png", Description = "State-of-the-art DDR5 memory combining premium aluminum construction, stunning custom LED lighting, and exceptional high-frequency performance to optimize modern desktop systems.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "G.Skill Trident Z5 RGB DDR5-6400 64GB", Brand = "G.Skill", Price = 10999, StockQuantity = 10, StockStatus = StockStatus.InStock, CategoryId = catRAM.Id, ImageUrl = "https://www.gskill.com/img/0/328/thumb-329-1.jpg", Description = "Ultra-high performance DDR5 memory kit designed for enthusiasts, featuring custom-curated ICs and a futuristic aluminum heatspreader with smooth, vibrant customizable RGB lighting.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "ASUS ROG Maximus Z790 Apex Encore", Brand = "ASUS", Price = 19999, StockQuantity = 4, StockStatus = StockStatus.LowStock, CategoryId = catMobo.Id, ImageUrl = "https://dlcdnwebimgs.asus.com/gain/6C6A7A6A-5E5E-4E4E-8E8E-1A1A2B2B3C3C/w717/h525", Description = "An elite motherboard designed specifically for record-breaking overclocking and extreme performance. Outfitted with robust power delivery, premium audio components, and cutting-edge WiFi 7 connectivity.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "MSI MEG X670E ACE", Brand = "MSI", Price = 17999, StockQuantity = 5, StockStatus = StockStatus.LowStock, CategoryId = catMobo.Id, ImageUrl = "https://asset.msi.com/resize/image/global/product/product_1661239914d53c3e3b3f0fc7e2b8e8c3e67b5e7b71.png62405b38c58fe0f07fcef2367d8a9ba1/1024.png", Description = "An enthusiast-level AM5 motherboard built to unlock the maximum potential of Ryzen 7000 and 9000 processors. Features exceptional power phases, robust thermal heatsinks, and comprehensive high-speed storage support.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Corsair HX1500i 80+ Platinum", Brand = "Corsair", Price = 6999, StockQuantity = 8, StockStatus = StockStatus.LowStock, CategoryId = catPSU.Id, ImageUrl = "https://www.corsair.com/medias/sys_master/images/images/h19/hf4/9109840330782/CP-9020215-NA-Gallery-HX1500i-PSU-01-CP-9020215-NA.png", Description = "A fully modular, ultra-high capacity power supply delivering pristine 80 PLUS Platinum certified power. Features digital control mapping and ATX 3.0 compliance for modern high-draw configurations.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Lian Li PC-O11 Dynamic EVO XL", Brand = "Lian Li", Price = 4999, StockQuantity = 10, StockStatus = StockStatus.InStock, CategoryId = catCase.Id, ImageUrl = "https://www.lian-li.com/wp-content/uploads/2022/06/O11DEXL-1.png", Description = "A legendary full-tower dual-chamber showcase case designed to accommodate extreme watercooling configurations and multiple massive radiators while putting your high-end build on gorgeous display.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "NZXT H9 Flow", Brand = "NZXT", Price = 3199, StockQuantity = 16, StockStatus = StockStatus.InStock, CategoryId = catCase.Id, ImageUrl = "https://nzxt.com/assets/cms/34299/1673040369-h9-flow-pdp-black-main.png", Description = "A premium mid-tower ATX case with dual-chamber design and mesh panels for exceptional airflow, perfect for high-performance builds.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "NZXT Kraken Elite 360 LCD", Brand = "NZXT", Price = 7499, StockQuantity = 8, StockStatus = StockStatus.LowStock, CategoryId = catCooler.Id, ImageUrl = "https://nzxt.com/assets/cms/34299/1643916217-kraken-elite-360-pdp-main.png", Description = "Premium 360mm AIO liquid cooler equipped with a high-resolution customizable LCD display on the pump head, allowing real-time system monitoring or custom GIF presentations.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Samsung 990 Pro 2TB PCIe 5.0 NVMe", Brand = "Samsung", Price = 5499, StockQuantity = 20, StockStatus = StockStatus.InStock, CategoryId = catStorage.Id, ImageUrl = "https://images.samsung.com/is/image/samsung/p6pim/global/mz-v9p2t0bw/gallery/global-990-pro-mz-v9p2t0bw-thumb-535669040", Description = "A blazing-fast NVMe SSD with PCIe 5.0 interface, ideal for professionals and gamers who demand maximum read/write speeds and large storage capacity.", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "WD Black SN850X 4TB NVMe", Brand = "WD", Price = 8999, StockQuantity = 12, StockStatus = StockStatus.InStock, CategoryId = catStorage.Id, ImageUrl = "https://m.media-amazon.com/images/I/61G4gTnASmL.jpg", Description = "A top-tier high-capacity NVMe drive engineered to eliminate loading times and deliver incredible gaming speeds. Outfitted with massive cache pools and robust endurance ratings.", IsActive = true }
            );

            await context.SaveChangesAsync();
        }
    }
}
