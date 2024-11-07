using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prn231Flower.Data.Models;
using prn231Flower.Repository.Interfaces;

namespace prn231Flower.API.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            if (order == null || order.OrderDetails == null || order.OrderDetails.Count == 0)
            {
                return BadRequest("Order details are required.");
            }

            var result = await _orderRepository.CreateOrderWithDetailsAsync(order);
            if (result > 0)
                return Ok("Order created successfully.");
            else
                return StatusCode(500, "An error occurred while creating the order.");
        }
    }
}
