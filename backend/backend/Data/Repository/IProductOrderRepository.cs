using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IProductOrderRepository
    {
        Task<IEnumerable<ProductOrderDto>> GetAllProductOrders();
        Task<ProductOrderDto> GetProductOrderById(Guid productId, Guid orderId);
        Task<ProductOrderDto> CreateProductOrder(ProductOrderCreateDto productOrderDto);
        // Task UpdateProductOrder(Guid productId, Guid orderId, ProductOrderUpdateDto productOrderDto);
        Task DeleteProductOrder(Guid productId, Guid orderId);
        Task<IEnumerable<ProductOrderDto>> GetAllProductOrders(int page, int pageSize, string sortBy, string sortOrder);
    }
}
