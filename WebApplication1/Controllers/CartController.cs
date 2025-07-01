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
    [Authorize(Roles = "User")]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CartController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{userId:Guid}")]
        public async Task<ActionResult<IEnumerable<CartItemReadDto>>> GetCartItems(Guid userId)
        {
            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == userId)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<CartItemReadDto>>(cartItems));
        }

        [HttpPost("{userId:Guid}")]
        public async Task<ActionResult> AddToCart(Guid userId, CartItemDto cartItemDto)
        {
            var existing = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == cartItemDto.ProductId);

            if (existing != null)
                existing.Quantity += cartItemDto.Quantity;
            else
            {
                var newItem = _mapper.Map<CartItem>(cartItemDto);
                newItem.UserId = userId;
                _context.CartItems.Add(newItem);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateCartItem(Guid id, [FromBody] int quatity)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            item.Quantity = quatity;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> RemoveCartItem(Guid id)
        {
            var item = await _context.CartItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
