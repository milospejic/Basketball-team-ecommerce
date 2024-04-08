﻿using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderDto>> GetAllOrders();
        Task<OrderDto> GetOrderById(Guid orderId);
        Task<Guid> CreateOrder(OrderCreateDto orderDto);
        Task UpdateOrder(Guid orderId, OrderUpdateDto orderDto);
        Task DeleteOrder(Guid orderId);
    }
}
