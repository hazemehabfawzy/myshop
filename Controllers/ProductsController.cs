using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechVault.API.DTOs.Product;
using TechVault.API.Services.Interfaces;

namespace TechVault.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetAll([FromQuery] Guid? categoryId, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] string? brand, [FromQuery] bool? inStock, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _productService.GetAllProductsAsync(categoryId, minPrice, maxPrice, brand, inStock, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult> Search([FromQuery] string q, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _productService.SearchProductsAsync(q, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("low-stock")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetLowStock([FromQuery] int threshold = 5)
        {
            var result = await _productService.GetLowStockProductsAsync(threshold);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseDto>> Create(CreateProductDto dto)
        {
            var result = await _productService.CreateProductAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductResponseDto>> Update(Guid id, UpdateProductDto dto)
        {
            var result = await _productService.UpdateProductAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
