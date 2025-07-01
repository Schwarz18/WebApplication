using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApplication2.Dtos;

namespace WebApplication2.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            try
            {
                var client = _httpClient.CreateClient("API");
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("/api/Auth/register", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Registration successful. Please login.";
                    return RedirectToAction("Login");
                }
                var error = await response.Content.ReadAsStringAsync();
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = _httpClient.CreateClient("API");
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await client.PostAsync("/api/Auth/login", content);
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(responseBody);

                    if (!string.IsNullOrEmpty(loginResponse?.JwtToken))
                    {   

                        HttpContext.Session.SetString("JWToken", loginResponse.JwtToken);

                        HttpContext.Session.SetString("UserId", loginResponse.UserId.ToString());


                        if (!string.IsNullOrEmpty(loginResponse.Role))
                        {
                            HttpContext.Session.SetString("UserRole", loginResponse.Role);
                        }

                        return RedirectToAction("Index", "Home");
                    }

                    HttpContext.Session.SetString("JWToken", loginResponse.JwtToken);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while processing your request.");
                return View(model);
            }
            
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Profile()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            var client = _httpClient.CreateClient("API");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"api/Auth/getUserProfile");

            if (!response.IsSuccessStatusCode) return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var profile = JsonConvert.DeserializeObject<UserProfileDto>(json);
            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var client = _httpClient.CreateClient("API");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"api/Auth/getUserProfile");
            if (!response.IsSuccessStatusCode) return View("Error");

            var json = await response.Content.ReadAsStringAsync();
            var profile = JsonConvert.DeserializeObject<UpdateUserProfileDto>(json);
            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(UpdateUserProfileDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("", "User is not authenticated.");
                return View(dto);
            }

            var client = _httpClient.CreateClient("API");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/Auth/updateUserProfile", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Update failed: {error}");
                return View(dto);
            }

            return RedirectToAction("Profile");
        }


        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var userId = HttpContext.Session.GetString("UserId");
            dto.Id = Guid.Parse(userId);

            var client = _httpClient.CreateClient("API");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"api/Auth/changePassword", content);
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Password change failed.");
                return View(dto);
            }

            return RedirectToAction("Profile");
        }

    }
}
