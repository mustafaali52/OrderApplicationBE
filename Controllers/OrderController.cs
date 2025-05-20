using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Core;
using OrderDeliverySystem.Core.Dtos;
using OrderDeliverySystem.Models;
using System.Security.Claims;

namespace OrderDeliverySystem.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : Controller
    {
        private readonly DataBaseContext _dataBaseContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public OrderController(DataBaseContext dataBaseContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _dataBaseContext = dataBaseContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IEnumerable<OrderDto>> GetOrders()
        {
            var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            var username = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            IQueryable<Order> query = _dataBaseContext.Orders;

            if (role == "Customer")
            {
                var user = await _dataBaseContext.Users.FirstAsync(u => u.UserName == username);
                query = query.Where(o => o.UserId == user.Id);
            }
            if (role == "Vendor")
            {
                var vendor = await _dataBaseContext.Users.FirstAsync(u => u.UserName == username);
                query = query.Where(o => o.Items.Any(oi => oi.Product.VendorId == vendor.Id));
            }

            return await query.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                UserId = o.UserId
            }).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _dataBaseContext.Orders.FindAsync(id);
            if (order == null) return NotFound();

            var role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            var username = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

            var currentUser = await _dataBaseContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (currentUser == null) return Unauthorized();

            if (role == "Customer" && order.UserId != currentUser.Id)
                return Forbid();

            if (role == "Vendor")
            {
                var isVendorAssociated = await _dataBaseContext.OrderItems
                    .AnyAsync(oi => oi.OrderId == id && oi.Product.VendorId == currentUser.Id);

                if (!isVendorAssociated)
                    return Forbid();
            }

            return Ok(new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                UserId = order.UserId
            });
        }


        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder(OrderDto orderInfo)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
            var user = await _dataBaseContext.Users.Where(x => x.UserName == userName)
               .Select(x => x).FirstOrDefaultAsync();

            var order = new Order
            {
                OrderDate = orderInfo.OrderDate,
                Status = "Pending",
                UserId = user.Id
            };
            _dataBaseContext.Add(order);
            await _dataBaseContext.SaveChangesAsync();

            var orderId = order.Id;

            var orderItems = new List<OrderItems>();
            foreach (var item in orderInfo.OrderListDto)
            {
                var orderItem = new OrderItems
                {
                    ProductId = item.ProductId,
                    Quatity = item.Quantity,
                    OrderId = orderId

                };
                orderItems.Add(orderItem);
            }
            _dataBaseContext.OrderItems.AddRange(orderItems);
            await _dataBaseContext.SaveChangesAsync();
            return Ok(order);

        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Vendor,Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var order = await _dataBaseContext.Orders.FindAsync(id);
            if (order == null) return NotFound();
            order.Status = status;
            await _dataBaseContext.SaveChangesAsync();
            return Ok(new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                UserId = order.UserId
            });
        }

    }
}
