using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OrderDeliverySystem.Core;
using OrderDeliverySystem.Core.Dtos;
using OrderDeliverySystem.Models;

namespace OrderDeliverySystem.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : Controller
    {
        private readonly DataBaseContext _dataBaseContext;
        public ProductController(DataBaseContext dataBaseContext) {
            _dataBaseContext = dataBaseContext;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductDto>> GetProducts() { 
            var products = await _dataBaseContext.Products
                .Select(x => x).ToListAsync();
            List<ProductDto> productDtos = new List<ProductDto>();
            foreach (var product in products) {
                productDtos.Add(new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    VendorId = product.VendorId
                });
            }
            return productDtos; 
        }

        [Authorize (Roles = "Admin,Vendor")]
        [HttpPost]
        public async Task<IActionResult> AddProduct (ProductDto productInfo)
        {
            var product = new Product
            {
                Name = productInfo.Name,
                Price = productInfo.Price,
                VendorId = productInfo.VendorId
            };
            _dataBaseContext.Products.Add(product);
            await _dataBaseContext.SaveChangesAsync();
            return Ok(new ProductDto { 
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                VendorId = product.VendorId
            });
        }
    }
}
