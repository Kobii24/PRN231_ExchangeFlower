using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prn231Flower.Data.Models;
using prn231Flower.Repository.Repositories;

namespace prn231Flower.API.Controllers;
public record LoginRequest(string Email, string Password);

[Route("api/[controller]")]
[ApiController]
public class Authentication : ControllerBase
{
    private readonly UserRepository _user;
    private readonly TokenRepository _token;

    public Authentication(UserRepository user, TokenRepository token)
    {
        _token = token;
        _user = user;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest account)
    {
        var user = AuthenticateUser(account.Email, account.Password);
        if (user != null)
        {
            var tokenString = _token.GenerateToken(user);
            return Ok(tokenString);
        }

        return Unauthorized("You are not allowed to access this function!");
    }

    private User AuthenticateUser(string EmailAddress, string Password)
    {
        var user = _user.GetAll().Where(u => u.Email.Equals(EmailAddress) 
        && BCrypt.Net.BCrypt.Verify(Password, u.Password)).FirstOrDefault();

        if(user != null)
            return user;

        return null;
    }
}
