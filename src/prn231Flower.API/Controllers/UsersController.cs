using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using prn231Flower.API.EmailServices;
using prn231Flower.API.Helper;
using prn231Flower.API.ViewModel;
using prn231Flower.Data.Models;
using prn231Flower.Repository.Repositories;

namespace prn231Flower.API.Controllers;
public record RegisterRequest(string Username, string Email, string Password,
    string Phone, string Address);

public record UpdateUserRequest(string Username, string Email, string Password,
    int Role, string Phone, string Address);

[Authorize(Policy = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserRepository _user;
    private readonly IEmailService _emailService;
    private readonly FlowerRepository _flower;
    private readonly OrderDetailRepository _orderDetail;
    private readonly NotificationRepository _notificate;

    public UsersController(UserRepository user, 
        IEmailService emailService, FlowerRepository flower, OrderDetailRepository orderDetail, NotificationRepository notificate)
    {
        _user = user;
        _emailService = emailService;
        _flower = flower;
        _orderDetail = orderDetail;
        _notificate = notificate;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUser()
    {
        var users = await _user.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("({Id})")]
    public async Task<ActionResult<User>> GetUserById([FromHeader]int Id)
    {
        var user = await _user.GetByIdAsync(Id);
        if (user is null)
            return NotFound($"Can not find user with {Id}");

        //var flowers = _flower.GetAll();
        //var listFlower = new List<Flower>();
        //foreach(var flower in flowers)
        //{
        //    if(flower.UserId == user.Id)
        //    {
        //        listFlower.Add(flower);
        //    }
        //}

        //var vm = new UserVM();

        //if (listFlower.Count == 0)
        //{
        //    return BadRequest("List flower is empty!");
        //}
        //else
        //{
        //    vm.Address = user.Address;
        //    vm.Username = user.Username;
        //    vm.Email = user.Email;
        //    vm.Flowers = listFlower;
        //}

        return Ok(user);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser([FromBody]RegisterRequest request)
    {
        var newUser = new User
        {
            Username = request.Username,
            Address = request.Address,
            Email = request.Email,
            Phone = request.Phone,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = 2,
            CreatedAt = DateTime.Now
        };
        await _user.CreateAsync(newUser);
        try
        {
            MailRequest mailRequest = new MailRequest();
            mailRequest.ToEmail = newUser.Email;
            mailRequest.Subject = "Welcome to Exchange Flower system " + newUser.Username;
            mailRequest.Body = "Register successfully! Welcome to Exchange Flower system " + newUser.Username;
            await _emailService.SendEmailAsync(mailRequest);
            return Ok("Go to email to confirm!");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    [HttpPut("({Id})")]
    public async Task<IActionResult> UpdateUser([FromHeader]int Id, [FromBody] UpdateUserRequest request)
    {
        var user = await _user.GetByIdAsync(Id);
        if (user is null)
            return BadRequest($"Can not find User with {Id} to update!");
        user.Email = request.Email;
        user.Phone = request.Phone;
        user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        user.Role = request.Role;
        user.Username = request.Username;
        user.Address = request.Address;

        _user.Update(user);
        await _user.SaveAsync();

        return Ok("IsSuccess");
    }

    [HttpDelete("({Id})")]
    public async Task<IActionResult> DeleteUser([FromHeader]int Id)
    {
        var user = await _user.GetByIdAsync(Id);
        if (user is null)
            return BadRequest($"Can not find User with {Id} to delete!");
        var flowers = _flower.GetAll();
        var listFlower = new List<Flower>();
        foreach (var flower in flowers)
        {
            if(flower.UserId == user.Id)
            {
                listFlower.Add(flower);
            }
        }

        foreach (var flower in listFlower)
        {
            foreach(var item in _orderDetail.GetAll())
            {
                if (item.FlowerId.Equals(flower.Id))
                {
                    _orderDetail.Remove(item);
                    await _orderDetail.SaveAsync();
                }

            }
        }

        foreach(var flower in listFlower)
        {
            if (flower.Id.Equals(user.Id))
            {
                _flower.Remove(flower);
                await _flower.SaveAsync();
            }
        }

        foreach(var item in _notificate.GetAll())
        {
            if (item.Id.Equals(user.Id))
            {
                _notificate.Remove(item);
                await _notificate.SaveAsync();
            }
        }

        _user.Remove(user);
        await _user.SaveAsync();
        return Ok("IsSuccess");
    }
}
