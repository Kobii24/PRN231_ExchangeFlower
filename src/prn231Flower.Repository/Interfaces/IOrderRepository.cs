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
    }
}
