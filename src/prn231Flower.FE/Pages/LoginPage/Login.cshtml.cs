using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using prn231Flower.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace prn231Flower.FE.Pages.LoginPage;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public IActionResult OnGetLogout()
    {
        HttpContext.Session.Remove("JWTToken");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var loginPayload = new User
        {
            Email = Email,
            Password = Password
        };

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("https://localhost:5050/api/Authentication/login", loginPayload);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            HttpContext.Session.SetString("JWTToken", token);
            return RedirectToPage("/Index");
        }

        ErrorMessage = "Please, entering exact email and password!";
        return Page();
    }
}
