
using Microsoft.AspNetCore.Mvc;
using prn231Flower.API.ViewModel;
using prn231Flower.Data.Models;
using prn231Flower.Repository.Repositories;

namespace prn231Flower.API.UserController;
public record RegisterRequest(string Username, string Email, string Password,
    int Role, string Phone, string Address);

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserRepository _user;

    public UsersController(UserRepository user)
    {
        _user = user;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUser()
    {
        var users = await _user.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("({Id})")]
    public async Task<ActionResult<UserVM>> GetUserById(int Id)
    {
        var user = await _user.GetByIdAsync(Id);
        if (user is null)
            return NotFound($"Can not find user with {Id}");
        var vn = new UserVM
        {
            Address = user.Address,
            Email = user.Email,
            Phone = user.Phone,
            Username = user.Username
        };
        return Ok(vn);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser(int Id, [FromBody] RegisterRequest request)
    {
        var user = await _user.GetByIdAsync(Id);
        if (user is not null)
            return BadRequest("User already exist!");
        var newUser = new User
        {
            Username = request.Username,
            Address = request.Address,
            Email = request.Email,
            Phone = request.Phone,
            Password = request.Password,
            Role = request.Role,
            CreatedAt = DateTime.Now
        };
        _user.Create(newUser);
        await _user.SaveAsync();
        return Ok(newUser);
    }

    [HttpPut("({Id})")]
    public async Task<IActionResult> UpdateUser(int Id, [FromBody] RegisterRequest request)
    {
        var user = await _user.GetByIdAsync(Id);
        if (user is null)
            return BadRequest($"Can not find User with {Id} to update!");
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.Password = request.Password;
        user.Role = request.Role;
        user.Username = request.Username;
        user.Address = request.Address;

        _user.Update(user);
        await _user.SaveAsync();

        return Ok("IsSuccess");
    }

    [HttpDelete("({Id})")]
    public async Task<IActionResult> DeleteUser(int Id)
    {
        var user = await _user.GetByIdAsync(Id);
        if (user is null)
            return BadRequest($"Can not find User with {Id} to delete!");
        _user.Remove(user);
        await _user.SaveAsync();
        return Ok("IsSuccess");
    }
}
