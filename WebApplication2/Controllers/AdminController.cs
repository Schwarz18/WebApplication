using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using WebApplication2.Dtos;
using System.Dynamic;

namespace WebApplication2.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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


        public async Task<IActionResult> GetAllUsers()
        {
            var client = CreateClientWithJwt();

            var response = await client.GetAsync("/api/Admin/");
            if (!response.IsSuccessStatusCode) return Unauthorized();

            var content = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<UserProfileDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(users);
        }

        public async Task<IActionResult> GetUserProfile(Guid id)
        {
            var client = CreateClientWithJwt();
            var response = await client.GetAsync($"/api/User/{id}");
            if (!response.IsSuccessStatusCode) return Unauthorized();
            var content = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserProfileDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return View(user);
        }

        

        //[HttpPost]
        //public async Task<IActionResult> DeleteUser(Guid id)
        //{
        //    var client = CreateClientWithJwt();
        //    var response = await client.DeleteAsync($"/api/User/{id}");
        //    if (response.IsSuccessStatusCode)
        //        return RedirectToAction("GetAllUsers");
        //    ModelState.AddModelError(string.Empty, "Failed to delete user.");
        //    return View();
        //}

        [HttpGet]
        public IActionResult Index()
        {
            // This method can be used to return a dashboard view for the admin
            return View();
        }

        //[HttpGet]
        //public IActionResult GetDashboard()
        //{
        //    // This method can be used to return a dashboard view for the admin
        //    // You can implement logic to fetch dashboard data here
        //    return View();
        //}

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var client = CreateClientWithJwt();
            var response = await client.GetAsync("api/Admin/Dashboard");
            if (!response.IsSuccessStatusCode)
                return View(new AdminDashboardDto());

            var content = await response.Content.ReadAsStringAsync();
            var dashboard = JsonSerializer.Deserialize<AdminDashboardDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(dashboard);
        }

    }
}
