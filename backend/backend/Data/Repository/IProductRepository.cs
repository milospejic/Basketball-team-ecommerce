using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetAllProducts();
        Task<ProductDto> GetProductById(Guid productId);
        Task<Guid> CreateProduct(ProductCreateDto productDto, Guid adminId);
        Task UpdateProduct(Guid productId, ProductUpdateDto productDto);
        Task DeleteProduct(Guid productId);
        
        Task<IEnumerable<ProductDto>> GetProductsByAdminId(Guid adminId);
        Task<IEnumerable<ProductDto>> GetProductsByName(string productName);
        Task<IEnumerable<ProductDto>> GetProductsByBrand(string brand);
        Task<IEnumerable<ProductDto>> GetProductsByCategory(string category);
        Task<IEnumerable<ProductDto>> GetProductsBySize(string size);

        Task<IEnumerable<ProductDto>> GetAllProducts(int page, int pageSize, string sortBy, string sortOrder);


    }
}
