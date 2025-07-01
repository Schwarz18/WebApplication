using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApplication2.Dtos;
using WebApplication2.Models;

namespace WebApplication1.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ProductController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private HttpClient CreateClientWithJwt()
        {
            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var client = CreateClientWithJwt();
            var response = await client.GetAsync("/api/Product/");
            if (!response.IsSuccessStatusCode) return Unauthorized();

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<ProductReadDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(products);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            var client = CreateClientWithJwt();
            var content = new StringContent(JsonSerializer.Serialize(productDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Product", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError(string.Empty, "Failed to create product.");
            return View(productDto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = CreateClientWithJwt();
            var response = await client.GetAsync($"/api/Product/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<ProductReadDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(product);
        }

        //[HttpPost]
        //public async Task<IActionResult> Edit(Guid id, ProductReadDto productDto)
        //{
        //    var client = _httpClientFactory.CreateClient("API");
        //    var token = HttpContext.Session.GetString("JWToken");
        //    var userId = HttpContext.Session.GetString("UserId");

        //    if (string.IsNullOrEmpty(userId))
        //        return View("Error", new ErrorViewModel { RequestId = "User ID not found in session." });

        //    if (token != null)
        //        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        //    productDto.Id = id; // Ensure ID is set

        //    var content = new StringContent(JsonSerializer.Serialize(productDto), Encoding.UTF8, "application/json");
        //    var response = await client.PutAsync($"/api/Product/{id}", content);

        //    if (response.IsSuccessStatusCode)
        //        return RedirectToAction("Index");

        //    var responseContent = await response.Content.ReadAsStringAsync();
        //    ModelState.AddModelError(string.Empty, $"Failed to update product. Server says: {responseContent}");
        //    return View(productDto);
        //}


        //[HttpGet]
        //public async Task<IActionResult> Edit(Guid id)
        //{
        //    var client = CreateClientWithJwt();
        //    var response = await client.GetAsync($"/api/Product/{id}");
        //    if (!response.IsSuccessStatusCode) return NotFound();

        //    var content = await response.Content.ReadAsStringAsync();
        //    var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //    return View(product);
        //}

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, ProductDto productDto)
        {
            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return View("Error",
                    new ErrorViewModel { RequestId = "User ID not found in session." });

            if (token != null)
                client.DefaultRequestHeaders.Authorization = new
                    System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(productDto), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"/api/Product/{id}", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError(string.Empty, "Failed to update product.");
            return View(productDto);
        }

        //[HttpGet]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var client = CreateClientWithJwt();
        //    var response = await client.GetAsync($"/api/Product/{id}");
        //    if (!response.IsSuccessStatusCode) return NotFound();

        //    var content = await response.Content.ReadAsStringAsync();
        //    var product = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //    return View(product);
        //}

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return View("Error",
                    new ErrorViewModel { RequestId = "User ID not found in session." });

            if (token != null)
                client.DefaultRequestHeaders.Authorization = new
                    System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"/api/Product/{id}");
            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            ModelState.AddModelError(string.Empty, "Failed to delete product.");
            return View();
        }
    }
}
