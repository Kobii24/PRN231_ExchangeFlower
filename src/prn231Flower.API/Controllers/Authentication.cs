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
        if (EmailAddress == "admin@gmail.com" && Password == "12345678")
        {
            var players = _user.GetAll();
            foreach (var player in players)
            {
                if (player.Email == "admin@gmail.com" && player.Password == "12345678")
                    return player;
            }
        }

        if (EmailAddress != "admin@gmail.com")
        {
            var listplayers = _user.GetAll();
            var tempPlayer = listplayers.Where(p
                => p.Email == EmailAddress && p.Password == Password).FirstOrDefault();
            if (tempPlayer != null)
                return tempPlayer;
        }

        return null;
    }
}
