using AutoMapper;
using backend.Data.Context;
using backend.Models.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Repository
{
    public class ProductOrderRepository : IProductOrderRepository
    {
        private readonly MyDbContext context;
        private readonly IMapper mapper;

        public ProductOrderRepository(MyDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ProductOrderDto>> GetAllProductOrders()
        {
            var productOrders = await context.ProductOrderTable.ToListAsync();
            return mapper.Map<IEnumerable<ProductOrderDto>>(productOrders);
        }

        public async Task<ProductOrderDto> GetProductOrderById(Guid orderId, Guid productId)
        {
            var productOrder = await context.ProductOrderTable.FindAsync(orderId, productId);
            return mapper.Map<ProductOrderDto>(productOrder);
        }

        public async Task CreateProductOrderAsync(ProductOrderCreateDto productOrderDto)
        {
            var productOrder = mapper.Map<ProductOrder>(productOrderDto);
            context.ProductOrderTable.Add(productOrder);
            await context.SaveChangesAsync();
        }

        public async Task UpdateProductOrder(Guid orderId, Guid productId, ProductOrderCreateDto productOrderDto)
        {
            var productOrder = await context.ProductOrderTable.FindAsync(orderId, productId);
            if (productOrder == null)
            {
                throw new ArgumentException("ProductOrder not found");
            }

            mapper.Map(productOrderDto, productOrder);
            await context.SaveChangesAsync();
        }

        public async Task DeleteProductOrder(Guid orderId, Guid productId)
        {
            var productOrder = await context.ProductOrderTable.FindAsync(orderId, productId);
            if (productOrder == null)
            {
                throw new ArgumentException("ProductOrder not found");
            }

            context.ProductOrderTable.Remove(productOrder);
            await context.SaveChangesAsync();
        }

        public Task<ProductOrderDto> GetProductOrderByIdAsync(Guid productOrderId)
        {
            throw new NotImplementedException();
        }

        Task<Guid> IProductOrderRepository.CreateProductOrder(ProductOrderCreateDto productOrderDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductOrderAsync(Guid productOrderId)
        {
            throw new NotImplementedException();
        }
    }

}
