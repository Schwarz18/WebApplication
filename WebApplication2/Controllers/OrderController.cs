using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApplication2.Dtos;
using WebApplication2.Models;

namespace WebApplication1.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            //var client = CreateClientWithJwt();
            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return View("Error",
                    new ErrorViewModel { RequestId = "User ID not found in session." });
            var response = await client.GetAsync($"api/Order/{userId}");
            if (!response.IsSuccessStatusCode) return Unauthorized();

            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderReadDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(orders);
        }


        public async Task<IActionResult> Orders()
        {
            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return View("Error",
                    new ErrorViewModel { RequestId = "User ID not found in session." });

            var response = await client.GetAsync("api/Order");
            if (!response.IsSuccessStatusCode)
                return Unauthorized();

            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderReadDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            foreach (var order in orders)
            {
                var orderedUserId = order.UserId;
                // Use orderedUserId as needed
            }
            return View(orders);
        }

        [HttpGet]
        public IActionResult PlaceOrder()
        {
            var items = new List<OrderItemDto>(); // or fetch from session/cart
            return View(items);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(List<OrderItemDto> items, string deliveryAddress, string paymentMethod)
        {
            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
                return View("Error", new ErrorViewModel { RequestId = "User ID not found in session." });

            var order = new CreateOrderDto
            {
                UserId = userId,
                DeliveryAddress = deliveryAddress,
                PaymentMethod = paymentMethod,
                OrderItems = items
            };

            var content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("api/Order", content);

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index", new { userId });

            ModelState.AddModelError(string.Empty, "Failed to place order.");
            return View("Index");
        }


        // GET: /Orders/EditStatus/{id}
        //public async Task<IActionResult> EditStatus(Guid id)
        //{
        //    var client = _httpClientFactory.CreateClient("API");
        //    var token = HttpContext.Session.GetString("JWToken");
        //    if (!string.IsNullOrEmpty(token))
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var userId = HttpContext.Session.GetString("UserId");
        //    if (string.IsNullOrEmpty(userId))
        //        return View("Error",
        //            new ErrorViewModel { RequestId = "User ID not found in session." });
        //    var order = await client.GetFromJsonAsync<OrderReadDto>($"api/Order/{id}");
        //    if (order == null)
        //        return NotFound();
        //    return View(order);
        //}

        //// POST: /Orders/EditStatus/{id}
        //[HttpPost]
        //public async Task<IActionResult> EditStatus(Guid id, string status)
        //{
        //    var client = _httpClientFactory.CreateClient("API");
        //    var token = HttpContext.Session.GetString("JWToken");
        //    if (!string.IsNullOrEmpty(token))
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    var userId = HttpContext.Session.GetString("UserId");
        //    if (string.IsNullOrEmpty(userId))
        //        return View("Error",
        //            new ErrorViewModel { RequestId = "User ID not found in session." });

        //    if (string.IsNullOrEmpty(status))
        //        return View("Error", new ErrorViewModel { RequestId = "Status cannot be null or empty." });

        //    var response = await client.PutAsJsonAsync($"api/Order/{id}/status", status);
        //    if (!response.IsSuccessStatusCode)
        //        ModelState.AddModelError("", "Failed to update status.");
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpGet]
        public async Task<IActionResult> EditStatus(Guid id)
        {
            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"api/Order/order/{id}");
            if (!response.IsSuccessStatusCode)
                return View("Error", new ErrorViewModel { RequestId = $"Failed to load order {id}." });

            var content = await response.Content.ReadAsStringAsync();
            var order = JsonSerializer.Deserialize<OrderReadDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (order == null)
                return View("Error", new ErrorViewModel { RequestId = $"Order {id} not found." });

            return View(order);
            //return PartialView("_EditStatusPartial", order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStatus(Guid id, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                ModelState.AddModelError("Status", "Status cannot be null or empty.");
                return View("Error", new ErrorViewModel { RequestId = "Status cannot be null or empty." });
            }

            var client = _httpClientFactory.CreateClient("API");
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.PutAsJsonAsync($"api/Order/{id}/status", status);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Failed to update status. API response: {errorContent}");
                    return View("Error", new ErrorViewModel { RequestId = "Failed to update status." });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Exception: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = ex.Message });
            }

            return RedirectToAction(nameof(Orders));
        }


    }
}
