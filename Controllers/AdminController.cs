using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Core;
using OrderDeliverySystem.Core.Dtos;
using System;

namespace OrderDeliverySystem.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DataBaseContext _dataBaseContext;
        public AdminController(DataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await _dataBaseContext.Users.Select(u => new UserDto
            {
                UserName = u.UserName,
                Password = "",
                Role = u.Role
            }).ToListAsync());
        }

        [HttpGet("all-orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            return Ok(await _dataBaseContext.Orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                Status = o.Status,
                UserId = o.UserId
            }).ToListAsync());
        }
    }
}
