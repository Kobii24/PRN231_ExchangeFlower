using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using prn231Flower.Data.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace prn231Flower.Repository.Repositories;

public class TokenRepository
{
    private readonly IConfiguration _conf;
    public TokenRepository(IConfiguration conf)
    {
        _conf = conf;
    }
    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenKey = Encoding.UTF8.GetBytes(_conf["JWT:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
          {
             new Claim(ClaimTypes.Email, user.Email),
             new Claim("password", user.Password),
             new Claim("role", user.Role.ToString()!),
             new Claim("Id", user.Id.ToString())
          }),
            Expires = DateTime.UtcNow.AddMinutes(10),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey),
            SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
