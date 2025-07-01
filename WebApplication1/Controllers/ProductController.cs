using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        //[Authorize(Roles ="User")]
        public ActionResult<IEnumerable<ProductReadDto>> GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(_mapper.Map<IEnumerable<ProductReadDto>>(products));
        }

        [HttpGet("{id:Guid}")]
        //[Authorize(Roles = "User")]
        public IActionResult GetProduct(Guid id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return Ok(_mapper.Map<ProductReadDto>(product));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<ProductReadDto> CreateProduct(ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            _context.Products.Add(product);
            _context.SaveChanges();

            var readDto = _mapper.Map<ProductReadDto>(product);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, readDto);
        }

        [HttpPut("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateProduct(Guid id, ProductDto productDto)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            _mapper.Map(productDto, product);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete([FromRoute] Guid id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
