using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderDto>> GetAllOrders();
        Task<OrderDto> GetOrderById(Guid orderId);
        Task<OrderDto> CreateOrder(OrderCreateDto orderDto, Guid userId);
        Task UpdateOrder(Guid orderId, OrderUpdateDto orderDto);
        Task DeleteOrder(Guid orderId);
        Task<IEnumerable<OrderDto>> GetOrdersByUserId(Guid userId);
        Task<IEnumerable<OrderDto>> GetAllOrders(int page, int pageSize, string sortBy, string sortOrder);
        Task PayOrder(Guid orderId);
    }
}
