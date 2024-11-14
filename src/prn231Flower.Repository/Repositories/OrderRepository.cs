using Microsoft.EntityFrameworkCore;
using prn231Flower.Data.Base;
using prn231Flower.Data.Models;
using prn231Flower.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prn231Flower.Repository.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(DatabaseContext context) : base(context) { }

        public async Task<int> CreateOrderWithDetailsAsync(Order order)
        {
            // Adds the order and related details in a single transaction
            _context.Orders.Add(order);
            foreach (var detail in order.OrderDetails)
            {
                _context.OrderDetails.Add(detail);
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            // Include OrderDetails for each order
            return await _context.Orders
                                 .Include(o => o.OrderDetails)
                                 .ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            // Retrieve a specific order by ID, including its details
            return await _context.Orders
                                 .Include(o => o.OrderDetails)
                                 .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await _context.Orders
                                       .Include(o => o.OrderDetails)
                                       .Include(o => o.Payments) // Include Payments
                                       .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return false;

            // Remove related Payments
            _context.Payments.RemoveRange(order.Payments);

            // Remove related OrderDetails
            _context.OrderDetails.RemoveRange(order.OrderDetails);

            // Remove the Order
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, int status)
        {
            var order = await _context.Orders.FindAsync(orderId);

            if (order == null)
            {
                return false;
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Order>> GetOrdersBySellerIdAsync(int sellerId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.OrderDetails.Any(od => _context.Flowers.Any(f => f.Id == od.FlowerId && f.UserId == sellerId)))
                .ToListAsync();
        }
    }
}
