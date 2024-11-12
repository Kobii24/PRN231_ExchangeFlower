using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using prn231Flower.Data.Models;
using prn231Flower.Repository.Repositories;

namespace prn231Flower.FE.Pages.FlowerPage
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserRepository _user;

        [BindProperty]
        public Flower Flower { get; set; }

        public CreateModel(IHttpClientFactory httpClientFactory, UserRepository user)
        {
            _httpClientFactory = httpClientFactory;
            _user = user;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWTToken");

            string userId = string.Empty;

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken != null)
            {
                var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "Id");
                if (roleClaim != null)
                {
                    userId = roleClaim.Value;
                }
            }

            Flower.UserId = int.Parse(userId);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("https://localhost:5050/api/Flowers", Flower);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");

            }
            return Page();
        }
    }
}
