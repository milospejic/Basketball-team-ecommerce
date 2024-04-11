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
            foreach (var order in orders)
            {
                var user = context.UserTable.FirstOrDefault(f => f.UserId == order.UserId);
                if (user != null)
                {
                    order.User = user;
                }
                var productOrders = await context.ProductOrderTable.ToListAsync();
                foreach(var productOrder in productOrders)
                {
                    var product = context.ProductTable.FirstOrDefault(f => f.ProductId == productOrder.ProductId);
                    if (product != null) {
                        order.Products.Add(mapper.Map<ProductDto>(product));
                    }
                }
            }
            return mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto> GetOrderById(Guid orderId)
        {
            var order = await context.OrderTable.FindAsync(orderId);
            if (order != null)
            {
                var user = context.UserTable.FirstOrDefault(f => f.UserId == order.UserId);
                if (user != null)
                {
                    order.User = user;
                }
            }
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
