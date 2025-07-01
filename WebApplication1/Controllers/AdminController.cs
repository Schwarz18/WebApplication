using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AdminController(ApplicationDbContext context, IMapper mapper) 
        {
            _context = context;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("Dashboard")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetDashboard()
        {
            var totalOrders = _context.Orders.Count();
            var totalProducts = _context.Products.Count();
            var totalUsers = _context.Users.Count();

            return Ok(new
            {
                TotalOrders = totalOrders,
                TotalProducts = totalProducts,
                TotalUsers = totalUsers
            });
        }

    }
}
