using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using prn231Flower.Data.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace prn231Flower.FE.Pages.CheckoutPage
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Name { get; set; }

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Phone { get; set; }

        [BindProperty]
        public string Address { get; set; }

        [BindProperty]
        public string Note { get; set; }

        [BindProperty]
        public int SelectedQuantity { get; set; }

        [BindProperty(SupportsGet = true)]
        public int FlowerId { get; set; }

        public Flower Flower { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            FlowerId = id; // Lưu FlowerId để sử dụng trong OnPostAsync
            await LoadFlowerDetailsAsync(id);
            return Page();
        }

        private async Task LoadFlowerDetailsAsync(int id)
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                ErrorMessage = "Please log in to view flower details.";
                return;
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"https://localhost:5050/api/Flowers/{id}");

            if (response.IsSuccessStatusCode)
            {
                Flower = await response.Content.ReadFromJsonAsync<Flower>();
            }
            else
            {
                Flower = null;
                ErrorMessage = "Flower details could not be loaded. Please try again.";
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra xem FlowerId có hợp lệ không
            if (FlowerId <= 0)
            {
                ErrorMessage = "Invalid Flower ID.";
                return Page();
            }

            // Load Flower details if not loaded
            if (Flower == null)
            {
                await LoadFlowerDetailsAsync(FlowerId);
                if (Flower == null)
                {
                    ErrorMessage = "Failed to retrieve flower details.";
                    return Page();
                }
            }

            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Decode JWT to get userId
            string userIdJWT = string.Empty;
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken != null)
            {
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id");
                if (roleClaim != null)
                {
                    userIdJWT = roleClaim.Value;
                }
            }

            // Tạo payload để gửi đến API
            var orderPayload = new
            {
                userId = userIdJWT,
                status = 0,
                note = Note,
                name = Name,
                email = Email,
                phone = Phone,
                address = Address,
                orderDetails = new[]
                {
                     new
                     {
                         flowerId = FlowerId,
                         quantity = SelectedQuantity,
                         description = Flower.Description,
                         totalPrice = Flower.Price * SelectedQuantity
                     }
                }
            };

            // Gửi yêu cầu POST đến API
            var response = await client.PostAsJsonAsync("https://localhost:5050/api/order", orderPayload);

            if (response.IsSuccessStatusCode)
            {
                // Chuyển hướng đến trang OrderPage sau khi tạo thành công
                return RedirectToPage("/OrderPage/Index");
            }

            ErrorMessage = "Failed to place order. Please try again.";
            return Page();
        }
    }
}
