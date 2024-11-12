using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NuGet.Common;
using prn231Flower.Data.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace prn231Flower.FE.Pages.SignUpPage;

public class SignUpModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SignUpModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [BindProperty]
    public string UserName { get; set; }

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    [BindProperty]
    public string Phone { get; set; }

    [BindProperty]
    public string Address { get; set; }

    public string ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var signUpPayload = new User
        {
            Username = UserName,
            Email = Email,
            Password = Password,
            Phone = Phone,
            Address = Address
        };
        var client = _httpClientFactory.CreateClient();

        var response = await client.PostAsJsonAsync("https://localhost:5050/api/Users/Register", signUpPayload);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToPage("/LoginPage/Login");
        }

        ErrorMessage = "User already exist!";
        return Page();
    }
}
