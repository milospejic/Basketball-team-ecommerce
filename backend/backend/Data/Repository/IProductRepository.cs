using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetAllProducts();
        Task<ProductDto> GetProductById(Guid productId);
        Task<Guid> CreateProduct(ProductCreateDto productDto);
        Task UpdateProduct(Guid productId, ProductUpdateDto productDto);
        Task DeleteProduct(Guid productId);
    }
}
