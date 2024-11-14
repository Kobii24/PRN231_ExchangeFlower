using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using prn231Flower.Data.Models;

namespace prn231Flower.FE.Pages.FlowerPage
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public List<Flower> Flowers { get; set; } = new List<Flower>();

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGetAsync(int? pageNumber)
        {
            var client = _httpClientFactory.CreateClient();
            var token = HttpContext.Session.GetString("JWTToken");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("Index");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("https://localhost:5050/api/Flowers");

            if (response.IsSuccessStatusCode)
            {
                Flowers = await response.Content.ReadFromJsonAsync<List<Flower>>();
                
                PageNumber = pageNumber ?? 1;
                TotalPages = (int)Math.Ceiling(Flowers!.Count / (double)PageSize);
                Flowers = Flowers.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            }

            return Page();
        }
    }
}
