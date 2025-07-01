using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public OrderController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("order/{id:Guid}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ActionResult<OrderReadDto>> GetOrderById(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound($"Order with ID {id} not found.");

            var orderDto = _mapper.Map<OrderReadDto>(order);
            return Ok(orderDto);
        }


        [HttpGet("{userId:Guid}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetUserOrders(Guid userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var orderDtos = _mapper.Map<List<OrderReadDto>>(orders);
            return Ok(orderDtos);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var orderDtos = _mapper.Map<List<OrderReadDto>>(orders);
            return Ok(orderDtos);
        }


        [HttpPost]
        [Authorize]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> PlaceOrder(CreateOrderDto orderDto)
        {
            var order = new Order
            {
                UserId = orderDto.UserId,
                OrderDate = DateTime.UtcNow,
                DeliveryAddress = orderDto.DeliveryAddress,
                PaymentMethod = orderDto.PaymentMethod,
                Status = "Pending",
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in orderDto.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return NotFound($"Product with ID {item.ProductId} not found.");
                }

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = product.Price
                });
            }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return Ok(new { order.Id });
        }



        [HttpPut("{id:Guid}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest("Status cannot be null or empty.");

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound($"Order with ID {id} not found.");
            }
            order.Status = status;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "An error occurred while updating the order status.");
            }
            return NoContent();
        }
    }
}
