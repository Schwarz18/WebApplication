using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApplication2.Dtos;
using WebApplication2.Models;

namespace WebApplication1.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CartController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        //private HttpClient CreateClientWithJwt()
        //{
        //    var client = _httpClientFactory.CreateClient("API");
        //    var token = HttpContext.Session.GetString("JWToken");
        //    var userId = HttpContext.Session.GetString("UserId");
        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    }
        //    return client;
        //}

        public async Task<IActionResult> Index()
        {
            //var client = CreateClientWithJwt();
            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return View("Error", new ErrorViewModel { RequestId = "User ID not found in session." });


            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new
                    System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"api/Cart/{userId}");
            if (!response.IsSuccessStatusCode) return Unauthorized();

            var content = await response.Content.ReadAsStringAsync();
            var cartItems = JsonSerializer.Deserialize<List<CartItemReadDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> Add(CartItemDto cartItemDto)
        {
            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            if (token != null)
                client.DefaultRequestHeaders.Authorization = new
                    System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonSerializer.Serialize(cartItemDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"api/Cart/{userId}", content);

            return RedirectToAction("Index", new { userId });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Guid id, int quantity)
        {
            //var client = CreateClientWithJwt();
            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return View("Error",
                    new ErrorViewModel { RequestId = "User ID not found in session." });

            if (token != null)
                client.DefaultRequestHeaders.Authorization = new
                    System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(quantity), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/Cart/{id}", content);

            return RedirectToAction("Index", new { userId });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid id)
        {
            //var client = CreateClientWithJwt();

            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return View("Error",
                    new ErrorViewModel { RequestId = "User ID not found in session." });

            if (token != null)
                client.DefaultRequestHeaders.Authorization = new
                    System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


            var response = await client.DeleteAsync($"api/Cart/{id}");

            return RedirectToAction("Index", new { userId });
        }
    }
}
