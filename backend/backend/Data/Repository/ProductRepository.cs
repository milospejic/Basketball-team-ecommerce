using AutoMapper;
using backend.Data.Context;
using backend.Models.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly MyDbContext context;
        private readonly IMapper mapper;

        public ProductRepository(MyDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            var products = await context.ProductTable.ToListAsync();
            return mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> GetProductById(Guid productId)
        {
            var product = await context.ProductTable.FindAsync(productId);
            return mapper.Map<ProductDto>(product);
        }

        public async Task<Guid> CreateProduct(ProductCreateDto productDto)
        {
            var product = mapper.Map<Product>(productDto);
            context.ProductTable.Add(product);
            await context.SaveChangesAsync();
            return product.ProductId;
        }

        public async Task UpdateProduct(Guid productId, ProductUpdateDto productDto)
        {
            var product = await context.ProductTable.FindAsync(productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            mapper.Map(productDto, product);
            await context.SaveChangesAsync();
        }

        public async Task DeleteProduct(Guid productId)
        {
            var product = await context.ProductTable.FindAsync(productId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            context.ProductTable.Remove(product);
            await context.SaveChangesAsync();
        }

    }

}
