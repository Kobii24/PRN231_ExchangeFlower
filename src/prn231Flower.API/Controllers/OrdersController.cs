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

        [HttpPost]
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

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound($"Order with ID {id} not found.");

            return Ok(order);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var isDeleted = await _orderRepository.DeleteOrderAsync(id);

            if (!isDeleted)
                return NotFound($"Order with ID {id} not found.");

            return Ok("Order deleted successfully.");
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            if (orders == null || !orders.Any())
            {
                return NotFound($"No orders found for user with ID {userId}.");
            }

            return Ok(orders);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] int status)
        {
            if (status < 0 || status > 4)
            {
                return BadRequest("Status must be an integer between 0 and 4.");
            }

            var isUpdated = await _orderRepository.UpdateOrderStatusAsync(id, status);

            if (!isUpdated)
            {
                return NotFound($"Order with ID {id} not found.");
            }

            return Ok("Order status updated successfully.");
        }

        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetOrdersBySellerId(int sellerId)
        {
            var orders = await _orderRepository.GetOrdersBySellerIdAsync(sellerId);
            if (orders == null || !orders.Any())
            {
                return NotFound($"No orders found for seller with ID {sellerId}.");
            }

            return Ok(orders);
        }

    }
}
