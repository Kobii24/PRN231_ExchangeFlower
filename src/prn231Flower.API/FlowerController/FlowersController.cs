﻿using Microsoft.AspNetCore.Mvc;
using prn231Flower.Data.Models;
using prn231Flower.Repository.Repositories;

namespace prn231Flower.API.FlowerController;
public record FlowerRequest(int UserId, string Type, int Quantity, 
    decimal Price, int Status, string Description, string ImgUrl);
public record UpdateRequest(int UserId, string Type, int Quantity,
    decimal Price, int Status, string Description, string ImgUrl);

[Route("api/[controller]")]
[ApiController]
public class FlowersController : ControllerBase
{
    private readonly FlowerRepository _flower;
    public FlowersController(FlowerRepository flower)
    {
        _flower = flower;
    }

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
        if(flower is null)
            return NotFound($"Can not find flower with {Id}");
        return Ok(flower);
    }

    [HttpPost]
    public async Task<IActionResult> CreateFlower([FromBody]FlowerRequest request)
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

    [HttpPut(("{Id}"))]
    public async Task<IActionResult> UpdateFlower(int Id, [FromBody]UpdateRequest request)
    {
        var flower = await _flower.GetByIdAsync(Id);
        if (flower is null)
            return NotFound($"Can not find flower with {Id} to update!");
        flower.UserId = request.UserId;
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

    [HttpDelete(("{Id}"))]
    public async Task<IActionResult> DeleteFlower(int Id)
    {
        var flower = await _flower.GetByIdAsync(Id);
        if (flower is null)
            return NotFound($"Can not find flower with {Id} to delete!");
        _flower.Remove(flower);
        await _flower.SaveAsync();
        return Ok("IsSuccess");
    }
}
