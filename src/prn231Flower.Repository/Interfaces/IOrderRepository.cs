using prn231Flower.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prn231Flower.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderWithDetailsAsync(Order order);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> GetOrderByIdAsync(int id);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<bool> UpdateOrderStatusAsync(int orderId, int status);
        Task<IEnumerable<Order>> GetOrdersBySellerIdAsync(int sellerId);
    }
}
