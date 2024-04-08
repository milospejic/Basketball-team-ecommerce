using AutoMapper;
using backend.Data.Context;
using backend.Models.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MyDbContext context;
        private readonly IMapper mapper;

        public OrderRepository(MyDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrders()
        {
            var orders = await context.OrderTable.ToListAsync();
            return mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderById(Guid orderId)
        {
            var order = await context.OrderTable.FindAsync(orderId);
            return mapper.Map<OrderDto>(order);
        }

        public async Task<Guid> CreateOrder(OrderCreateDto orderDto)
        {
            var order = mapper.Map<Order>(orderDto);
            context.OrderTable.Add(order);
            await context.SaveChangesAsync();
            return order.OrderId;
        }

        public async Task UpdateOrder(Guid orderId, OrderUpdateDto orderDto)
        {
            var order = await context.OrderTable.FindAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            mapper.Map(orderDto, order);
            await context.SaveChangesAsync();
        }

        public async Task DeleteOrder(Guid orderId)
        {
            var order = await context.OrderTable.FindAsync(orderId);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            context.OrderTable.Remove(order);
            await context.SaveChangesAsync();
        }
    }

}
