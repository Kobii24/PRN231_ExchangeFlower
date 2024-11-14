using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prn231Flower.Data.Models;
using prn231Flower.Repository.Repositories;

namespace prn231Flower.API.Controllers;
public record FlowerRequest(int UserId, string Type, int Quantity,
    decimal Price, int Status, string Description, string ImgUrl);
public record UpdateRequest(string Type, int Quantity,
    decimal Price, int Status, string Description, string ImgUrl);

[Route("api/[controller]")]
[ApiController]
public class FlowersController : ControllerBase
{
    private readonly FlowerRepository _flower;
    private readonly OrderDetailRepository _od;
    public FlowersController(FlowerRepository flower, OrderDetailRepository od)
    {
        _flower = flower;
        _od = od;
    }

    [Authorize("AllRoles")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Flower>>> GetAllFlower()
    {
        var flowers = await _flower.GetAllAsync();
        return Ok(flowers);
    }

    [HttpGet("({Id})")]
    public async Task<ActionResult<Flower>> GetFlowerById(int Id)
    {
        var flower = await _flower.GetByIdAsync(Id);
        if (flower is null)
            return NotFound($"Can not find flower with {Id}");
        return Ok(flower);
    }

    [Authorize("AdminAndSeller")]
    [HttpPost]
    public async Task<IActionResult> CreateFlower([FromBody] FlowerRequest request)
    {
        var flower = new Flower
        {
            UserId = request.UserId,
            Type = request.Type,
            Quantity = request.Quantity,
            Price = request.Price,
            Status = request.Status,
            Description = request.Description,
            ImgUrl = request.ImgUrl,
            CreatedAt = DateTime.Now
        };
        _flower.Create(flower);
        await _flower.SaveAsync();
        return Ok(flower);
    }

    [Authorize("AdminAndSeller")]
    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateFlower([FromHeader]int Id, [FromBody] UpdateRequest request)
    {
        var flower = await _flower.GetByIdAsync(Id);
        if (flower is null)
            return NotFound($"Can not find flower with {Id} to update!");
        flower.Type = request.Type;
        flower.Quantity = request.Quantity;
        flower.Price = request.Price;
        flower.Status = request.Status;
        flower.Description = request.Description;
        flower.ImgUrl = request.ImgUrl;

        _flower.Update(flower);
        await _flower.SaveAsync();
        return Ok("IsSuccess");
    }

    [Authorize("AdminAndSeller")]
    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteFlower([FromHeader]int Id)
    {
        var flower = await _flower.GetByIdAsync(Id);
        if (flower is null)
            return NotFound($"Can not find flower with {Id} to delete!");
        
        foreach (var item in _od.GetAll())
        {
            if (item.FlowerId.Equals(flower.Id))
            {
                _od.Remove(item);
                await _od.SaveAsync();
            }
        }

        _flower.Remove(flower);
        await _flower.SaveAsync();
        return Ok("IsSuccess");
    }
}
