using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using prn231Flower.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace prn231Flower.FE.Pages.OrderPage
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<Order> Orders { get; set; } = new List<Order>();
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Decode JWT to get userId
            string userId = GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                ErrorMessage = "User ID not found in token.";
                return Page();
            }

            // Fetch orders by userId
            var response = await client.GetAsync($"https://localhost:5050/api/order/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                Orders = await response.Content.ReadFromJsonAsync<List<Order>>();
            }
            else
            {
                ErrorMessage = "Failed to load orders. Please try again.";
            }

            return Page();
        }

        private string GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            return jwtToken?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        }

        public string GetStatusText(int status)
        {
            return status switch
            {
                0 => "Pending",
                1 => "Confirmed",
                2 => "Completed",
                3 => "Cancelled",
                _ => "Unknown"
            };
        }
    }
}
