using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prn231Flower.API.ViewModel;
using prn231Flower.Data.Models;

namespace prn231Flower.FE.Pages.UserPage;

public class EditModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    [BindProperty]
    public User User { get; set; }

    public EditModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;

    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var client = _httpClientFactory.CreateClient();
        var token = HttpContext.Session.GetString("JWTToken");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        client.DefaultRequestHeaders.Add("Id",id.ToString());

        var response = await client.GetAsync($"https://localhost:5050/api/Users/({id})");
        if (response.IsSuccessStatusCode)
        {
            User = await response.Content.ReadFromJsonAsync<User>();
            return Page();
        }
        return NotFound();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = _httpClientFactory.CreateClient();
        var token = HttpContext.Session.GetString("JWTToken");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        client.DefaultRequestHeaders.Add("Id", User.Id.ToString());

        var response = await client.PutAsJsonAsync($"https://localhost:5050/api/Users/({User.Id})", User);
        if (response.IsSuccessStatusCode)
        {
            return RedirectToPage("Index");
        }
        return Page();
    }
}
