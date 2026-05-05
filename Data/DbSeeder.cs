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
            var adminHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
            var techHash = BCrypt.Net.BCrypt.HashPassword("Tech123!");
            var userHash = BCrypt.Net.BCrypt.HashPassword("User123!");

            var adminUser = new User { Id = Guid.NewGuid(), Username = "admin", Email = "admin@techvault.com", PasswordHash = adminHash, Role = "Admin" };
            var techUser = new User { Id = Guid.NewGuid(), Username = "tech1", Email = "tech1@techvault.com", PasswordHash = techHash, Role = "Technician" };
            var johnUser = new User { Id = Guid.NewGuid(), Username = "john", Email = "john@example.com", PasswordHash = userHash, Role = "Customer" };

            context.Users.AddRange(adminUser, techUser, johnUser);
            
            context.CustomerProfiles.AddRange(
                new CustomerProfile { Id = Guid.NewGuid(), UserId = adminUser.Id, FullName = "System Admin" },
                new CustomerProfile { Id = Guid.NewGuid(), UserId = techUser.Id, FullName = "Tech Lead" },
                new CustomerProfile { Id = Guid.NewGuid(), UserId = johnUser.Id, FullName = "John Doe", PhoneNumber = "555-0199", Address = "Tech City, 42" }
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
                // GRAPHICS CARDS:
                new Product { Id = Guid.NewGuid(), Name = "ASUS ROG STRIX RTX 4090 OC 24GB", Brand = "ASUS", Price = 54999, StockQuantity = 4, CategoryId = catGPU.Id, ImageUrl = "https://dlcdnwebimgs.asus.com/gain/A5C97B8A-D93F-4B36-9A0A-FBD7BD19D6F6/w717/h525", Description = "24GB GDDR6X, 2640MHz boost, triple ARGB fans, PCIe 4.0 x16", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "MSI RTX 4080 Super 16G Gaming X Slim", Brand = "MSI", Price = 34999, StockQuantity = 7, CategoryId = catGPU.Id, ImageUrl = "https://asset.msi.com/resize/image/global/product/product_17001399478f2f36da3ab19eb29fefc1b63eafc3fa.png62405b38c58fe0f07fcef2367d8a9ba1/1024.png", Description = "16GB GDDR6X, 2610MHz boost, slim dual-fan design, DLSS 3", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Sapphire NITRO+ RX 7900 XTX 24GB", Brand = "Sapphire", Price = 28999, StockQuantity = 5, CategoryId = catGPU.Id, ImageUrl = "https://www.sapphiretech.com/productimg/?image=aHR0cHM6Ly93d3cuc2FwcGhpcmV0ZWNoLmNvbS9tZWRpYWxpYi9wcm9kdWN0L3ByZW1pdW0vMTMzMDIvbWFpbl8xLmpwZw==", Description = "24GB GDDR6, 2615MHz boost, RDNA 3, DisplayPort 2.1", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "ASUS ROG STRIX RTX 4070 Ti Super OC", Brand = "ASUS", Price = 22999, StockQuantity = 10, CategoryId = catGPU.Id, ImageUrl = "https://dlcdnwebimgs.asus.com/gain/5D7E7C9F-E97A-4D15-B5B3-DECF7A6BD5D2/w717/h525", Description = "16GB GDDR6X, 2670MHz boost, DLSS 3.5, triple fan cooling", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Gigabyte RX 7800 XT Gaming OC 16GB", Brand = "Gigabyte", Price = 14999, StockQuantity = 15, CategoryId = catGPU.Id, ImageUrl = "https://static.gigabyte.com/StaticFile/Image/Global/b11855b5e8e3bff28c6b90e85d08edd4/Product/30437/png", Description = "16GB GDDR6, 2430MHz boost, WINDFORCE 3X cooling, 1080p-1440p king", IsActive = true },

                // PROCESSORS:
                new Product { Id = Guid.NewGuid(), Name = "Intel Core i9-14900K", Brand = "Intel", Price = 14999, StockQuantity = 12, CategoryId = catCPU.Id, ImageUrl = "https://www.intel.com/content/dam/www/central-libraries/us/en/images/2022-11/processors-core-i9-13900k-front-angle-rwd.png.rendition.intel.web.550.309.png", Description = "24 cores (8P+16E), 6.0GHz boost, LGA1700, 125W TDP, DDR5/DDR4", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "AMD Ryzen 9 7950X3D", Brand = "AMD", Price = 16999, StockQuantity = 9, CategoryId = catCPU.Id, ImageUrl = "https://www.amd.com/content/dam/amd/en/images/products/processors/ryzen/2179260-ryzen-9-7950x3d-PIB-1260x709.png", Description = "16 cores, 5.7GHz boost, AM5, 144MB 3D V-Cache, workstation + gaming", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Intel Core i7-14700K", Brand = "Intel", Price = 9999, StockQuantity = 18, CategoryId = catCPU.Id, ImageUrl = "https://www.intel.com/content/dam/www/central-libraries/us/en/images/2022-11/processors-core-i7-13700k-front-angle-rwd.png.rendition.intel.web.550.309.png", Description = "20 cores (8P+12E), 5.6GHz boost, LGA1700, best mid-range value", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "AMD Ryzen 7 7800X3D", Brand = "AMD", Price = 9499, StockQuantity = 22, CategoryId = catCPU.Id, ImageUrl = "https://www.amd.com/content/dam/amd/en/images/products/processors/ryzen/2179262-ryzen-7-7800x3d-PIB-1260x709.png", Description = "8 cores, 5.0GHz boost, AM5, 96MB 3D V-Cache, #1 gaming CPU", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "AMD Ryzen 5 7600X", Brand = "AMD", Price = 5499, StockQuantity = 30, CategoryId = catCPU.Id, ImageUrl = "https://www.amd.com/content/dam/amd/en/images/products/processors/ryzen/2179273-ryzen-5-7600x-PIB-1260x709.png", Description = "6 cores, 5.3GHz boost, AM5, DDR5, excellent budget pick", IsActive = true },

                // RAM:
                new Product { Id = Guid.NewGuid(), Name = "Corsair Dominator Titanium DDR5-6000 32GB", Brand = "Corsair", Price = 5999, StockQuantity = 25, CategoryId = catRAM.Id, ImageUrl = "https://www.corsair.com/medias/sys_master/images/images/hf8/h39/10526826471454/CMP32GX5M2B6000C30-Gallery-Dominator-Titanium-RGB-DDR5-6000MHZ-32GB-2X16GB-Black-01-CMP32GX5M2B6000C30.png", Description = "2x16GB, DDR5-6000, CL30, EXPO/XMP 3.0, RGB, PMIC on die", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "G.Skill Trident Z5 RGB DDR5-6400 64GB", Brand = "G.Skill", Price = 10999, StockQuantity = 10, CategoryId = catRAM.Id, ImageUrl = "https://www.gskill.com/img/0/328/thumb-329-1.jpg", Description = "2x32GB, DDR5-6400, CL32, XMP 3.0, symmetric RGB diffuser", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Kingston Fury Beast DDR5-5200 32GB", Brand = "Kingston", Price = 3999, StockQuantity = 35, CategoryId = catRAM.Id, ImageUrl = "https://media.kingston.com/kingston/product/ktc-product-fury-beast-ddr5-rgb-kf552c40bbak2-32-1-zm.jpg", Description = "2x16GB, DDR5-5200, CL40, XMP 3.0 / EXPO, plug & play", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "G.Skill Ripjaws V DDR4-3600 16GB", Brand = "G.Skill", Price = 1999, StockQuantity = 50, CategoryId = catRAM.Id, ImageUrl = "https://www.gskill.com/img/0/165/thumb-166-1.jpg", Description = "2x8GB, DDR4-3600, CL16, budget DDR4 champion", IsActive = true },

                // MOTHERBOARDS:
                new Product { Id = Guid.NewGuid(), Name = "ASUS ROG Maximus Z790 Apex Encore", Brand = "ASUS", Price = 19999, StockQuantity = 4, CategoryId = catMobo.Id, ImageUrl = "https://dlcdnwebimgs.asus.com/gain/6C6A7A6A-5E5E-4E4E-8E8E-1A1A2B2B3C3C/w717/h525", Description = "LGA1700, Z790, DDR5 OC, WiFi 7, 10G LAN, PCIe 5.0 x16", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "MSI MEG X670E ACE", Brand = "MSI", Price = 17999, StockQuantity = 5, CategoryId = catMobo.Id, ImageUrl = "https://asset.msi.com/resize/image/global/product/product_1661239914d53c3e3b3f0fc7e2b8e8c3e67b5e7b71.png62405b38c58fe0f07fcef2367d8a9ba1/1024.png", Description = "AM5, X670E, PCIe 5.0 M.2 x3, WiFi 6E, 10G LAN, premium VRM", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "ASUS ProArt Z790 Creator WiFi", Brand = "ASUS", Price = 12999, StockQuantity = 7, CategoryId = catMobo.Id, ImageUrl = "https://dlcdnwebimgs.asus.com/gain/1B1B2C2C-3D3D-4E4E-5F5F-6A6A7B7B8C8C/w717/h525", Description = "LGA1700, Z790, Thunderbolt 4, WiFi 6E, creator-focused", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Gigabyte B650 AORUS Elite AX", Brand = "Gigabyte", Price = 6999, StockQuantity = 15, CategoryId = catMobo.Id, ImageUrl = "https://static.gigabyte.com/StaticFile/Image/Global/2af87fd18df4b31e5e258e77b7bb3eb0/Product/29897/png", Description = "AM5, B650, DDR5, WiFi 6E, PCIe 5.0 M.2, great value AM5 board", IsActive = true },

                // POWER SUPPLIES:
                new Product { Id = Guid.NewGuid(), Name = "Corsair HX1500i 80+ Platinum", Brand = "Corsair", Price = 6999, StockQuantity = 8, CategoryId = catPSU.Id, ImageUrl = "https://www.corsair.com/medias/sys_master/images/images/h19/hf4/9109840330782/CP-9020215-NA-Gallery-HX1500i-PSU-01-CP-9020215-NA.png", Description = "1500W, 80+ Platinum, fully modular, ATX 3.0, PCIe 5.0 native", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Seasonic Prime TX-1000 80+ Titanium", Brand = "Seasonic", Price = 5499, StockQuantity = 6, CategoryId = catPSU.Id, ImageUrl = "https://seasonic.com/pub/media/catalog/product/cache/1/image/800x800/9df78eab33525d08d6e5fb8d27136e95/p/r/prime-tx-1000_1.jpg", Description = "1000W, 80+ Titanium, 12-year warranty, fanless under 20% load", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "be quiet! Dark Power 13 850W", Brand = "be quiet!", Price = 4499, StockQuantity = 12, CategoryId = catPSU.Id, ImageUrl = "https://www.bequiet.com/remote/media/products/800/1922_z_800x800.png", Description = "850W, 80+ Titanium, fully modular, overclocking switch, whisper quiet", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "EVGA SuperNOVA 750 G6 80+ Gold", Brand = "EVGA", Price = 2999, StockQuantity = 20, CategoryId = catPSU.Id, ImageUrl = "https://images.evga.com/products/gallery/png/220-G6-0750-X1_LG_1.png", Description = "750W, 80+ Gold, fully modular, 10-year warranty, compact 140mm", IsActive = true },

                // PC CASES:
                new Product { Id = Guid.NewGuid(), Name = "Lian Li PC-O11 Dynamic EVO XL", Brand = "Lian Li", Price = 4999, StockQuantity = 10, CategoryId = catCase.Id, ImageUrl = "https://www.lian-li.com/wp-content/uploads/2022/06/O11DEXL-1.png", Description = "Full tower, dual chamber, 3x360mm rad support, dual TG panels", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Fractal Design Torrent RGB", Brand = "Fractal", Price = 3999, StockQuantity = 12, CategoryId = catCase.Id, ImageUrl = "https://www.fractal-design.com/app/uploads/2022/09/Torrent-RGB-Black-TG-Dark_front.png", Description = "Mid tower, max airflow, 2x180mm+3x120mm RGB fans included", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Corsair iCUE 5000X RGB", Brand = "Corsair", Price = 3499, StockQuantity = 14, CategoryId = catCase.Id, ImageUrl = "https://www.corsair.com/medias/sys_master/images/images/h30/h59/8975956541470/CC-9011212-WW-Gallery-iCUE-5000X-RGB-Mid-Tower-ATX-PC-Smart-Case-Black-01-CC-9011212-WW.png", Description = "Mid tower, ATX, 3x TG panels, 3x120mm iCUE RGB fans included", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "NZXT H9 Flow", Brand = "NZXT", Price = 3199, StockQuantity = 16, CategoryId = catCase.Id, ImageUrl = "https://nzxt.com/assets/cms/34299/1673040369-h9-flow-pdp-black-main.png", Description = "Mid tower, dual-chamber, 4x TG panels, excellent airflow", IsActive = true },

                // CPU COOLERS:
                new Product { Id = Guid.NewGuid(), Name = "NZXT Kraken Elite 360 LCD", Brand = "NZXT", Price = 7499, StockQuantity = 8, CategoryId = catCooler.Id, ImageUrl = "https://nzxt.com/assets/cms/34299/1643916217-kraken-elite-360-pdp-main.png", Description = "360mm AIO, 2.36-inch LCD head, 3x120mm Aer RGB, LGA1700/AM5", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Corsair iCUE H150i Elite Capellix XT", Brand = "Corsair", Price = 5999, StockQuantity = 10, CategoryId = catCooler.Id, ImageUrl = "https://www.corsair.com/medias/sys_master/images/images/hac/hdd/10448880754718/CW-9060065-WW-Gallery-iCUE-H150i-ELITE-CAPELLIX-XT-Liquid-CPU-Cooler-01-CW-9060065-WW.png", Description = "360mm AIO, iCUE Commander Core, 3x120mm RGB fans, LGA1700/AM5", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Noctua NH-D15 chromax.black", Brand = "Noctua", Price = 2999, StockQuantity = 18, CategoryId = catCooler.Id, ImageUrl = "https://noctua.at/pub/media/wysiwyg/products/nh-d15-chromax-black/gallery/nh_d15_chromax_black_1.jpg", Description = "Dual tower air cooler, 2x140mm NF-A15, 280W+ TDP support", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "DeepCool LT720 360mm AIO", Brand = "DeepCool", Price = 4499, StockQuantity = 11, CategoryId = catCooler.Id, ImageUrl = "https://asset.deepcool.com/2022/09/lt720/lt720-01.jpg", Description = "360mm AIO, infinity mirror head, 3x120mm FK120, AM5/LGA1700", IsActive = true },

                // STORAGE:
                new Product { Id = Guid.NewGuid(), Name = "Samsung 990 Pro 2TB PCIe 5.0 NVMe", Brand = "Samsung", Price = 5499, StockQuantity = 20, CategoryId = catStorage.Id, ImageUrl = "https://images.samsung.com/is/image/samsung/p6pim/global/mz-v9p2t0bw/gallery/global-990-pro-mz-v9p2t0bw-thumb-535669040", Description = "2TB, 7450/6900 MB/s, M.2 2280, TLC NAND, PCIe 4.0 x4", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "WD Black SN850X 4TB NVMe", Brand = "WD", Price = 8999, StockQuantity = 12, CategoryId = catStorage.Id, ImageUrl = "https://m.media-amazon.com/images/I/61G4gTnASmL.jpg", Description = "4TB, 7300/6600 MB/s, PCIe 4.0, PS5 compatible, Game Mode 2.0", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Seagate FireCuda 530 2TB NVMe", Brand = "Seagate", Price = 4999, StockQuantity = 16, CategoryId = catStorage.Id, ImageUrl = "https://www.seagate.com/files/www-content/product-content/firecuda-fam/firecuda-530/_shared/images/firecuda-530-top-angle-1200x1200.png", Description = "2TB, 7300/6900 MB/s, PCIe 4.0, optional heatsink, PS5 ready", IsActive = true },

                // MONITORS:
                new Product { Id = Guid.NewGuid(), Name = "LG UltraGear 27GP950-B 4K 144Hz", Brand = "LG", Price = 12999, StockQuantity = 8, CategoryId = catMon.Id, ImageUrl = "https://gscs-b2c.lge.com/downloadFile?fileId=UWSwCBMRWbEfYa6aWBMiJA", Description = "27-inch, 4K IPS, 144Hz, G-Sync Compatible, 1ms GtG, HDMI 2.1", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Samsung Odyssey G7 32-inch 240Hz", Brand = "Samsung", Price = 9999, StockQuantity = 10, CategoryId = catMon.Id, ImageUrl = "https://images.samsung.com/is/image/samsung/p6pim/levant/lc32g75tqsmxzn/gallery/levant-odyssey-g7-c32g75tqss-lc32g75tqsmxzn-thumb-368498918", Description = "32-inch QHD VA, 240Hz, 1ms, 1000R curve, G-Sync+FreeSync", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "ASUS ROG Swift PG279QM 240Hz", Brand = "ASUS", Price = 11999, StockQuantity = 6, CategoryId = catMon.Id, ImageUrl = "https://dlcdnwebimgs.asus.com/gain/BD8B5A9C-A15A-4A86-8C3A-8A7ADFBDCA72/w717/h525", Description = "27-inch QHD IPS, 240Hz, 1ms, G-Sync Ultimate, HDR600", IsActive = true },

                // PERIPHERALS:
                new Product { Id = Guid.NewGuid(), Name = "Logitech G Pro X Superlight 2", Brand = "Logitech", Price = 2499, StockQuantity = 30, CategoryId = catPeri.Id, ImageUrl = "https://resource.logitech.com/content/dam/gaming/en/products/pro-x-superlight/pro-x-superlight-gallery-1.png", Description = "60g wireless gaming mouse, HERO 25K sensor, 95hr battery", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Corsair K100 RGB Mechanical Keyboard", Brand = "Corsair", Price = 3499, StockQuantity = 20, CategoryId = catPeri.Id, ImageUrl = "https://www.corsair.com/medias/sys_master/images/images/h89/h71/8975941222430/CH-912A01A-NA-Gallery-K100-RGB-Mechanical-Gaming-Keyboard-01-CH-912A01A-NA.png", Description = "OPX optical-mechanical switches, iCUE, 44-zone RGB, media wheel", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "SteelSeries Arctis Nova Pro Wireless", Brand = "SteelSeries", Price = 3999, StockQuantity = 15, CategoryId = catPeri.Id, ImageUrl = "https://steelseries.com/api/assets/images/products/arctis-nova-pro-wireless/hero", Description = "Dual wireless, ANC, hi-fi audio, hot-swap battery system", IsActive = true },

                // FLAGSHIP PHONES:
                new Product { Id = Guid.NewGuid(), Name = "Samsung Galaxy S24 Ultra", Brand = "Samsung", Price = 52999, StockQuantity = 15, CategoryId = catPhone.Id, ImageUrl = "https://images.samsung.com/is/image/samsung/p6pim/levant/2401/gallery/levant-galaxy-s24-ultra-s928-sm-s928bztgmid-thumb-539573051", Description = "6.8-inch AMOLED, S Pen, 200MP, Snapdragon 8 Gen 3, 5000mAh", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "iPhone 15 Pro Max", Brand = "Apple", Price = 58999, StockQuantity = 10, CategoryId = catPhone.Id, ImageUrl = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-15-pro-finish-select-202309-6-7inch-naturaltitanium", Description = "6.7-inch Super Retina XDR, A17 Pro, 48MP ProRAW, titanium", IsActive = true },

                // GAMING LAPTOPS:
                new Product { Id = Guid.NewGuid(), Name = "ASUS ROG Strix G16 2024", Brand = "ASUS", Price = 67999, StockQuantity = 6, CategoryId = catLaptop.Id, ImageUrl = "https://dlcdnwebimgs.asus.com/gain/7AC7EDB8-2643-4D97-BE2A-38FFFE56E3AC/w717/h525", Description = "16-inch QHD 240Hz, i9-14900HX, RTX 4080, 32GB DDR5, 1TB", IsActive = true },
                new Product { Id = Guid.NewGuid(), Name = "Lenovo Legion Pro 7i Gen 9", Brand = "Lenovo", Price = 71999, StockQuantity = 5, CategoryId = catLaptop.Id, ImageUrl = "https://p3-ofp.static.pub/fes/cms/2024/01/10/ofp-p3-ofp-v3.lenovo.com/products/laptops/legion-series/legion-7i-gen-9/gallery/lenovo-laptop-legion-7i-gen-9-16-intel-luna-grey-gallery-1.png", Description = "16-inch IPS 240Hz, i9-14900HX, RTX 4080, 32GB DDR5, 2TB SSD", IsActive = true }
            );

            await context.SaveChangesAsync();
        }
    }
}
