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

        public async Task<ProductOrderDto> GetProductOrderById(Guid productId, Guid orderId)
        {
            var productOrder = await context.ProductOrderTable.FindAsync(orderId, productId);
            return mapper.Map<ProductOrderDto>(productOrder);
        }

        public async Task<ProductOrderDto> CreateProductOrder(ProductOrderCreateDto productOrderDto)
        {
            var productOrder = mapper.Map<ProductOrder>(productOrderDto);
            context.ProductOrderTable.Add(productOrder);
            var product = context.ProductTable.FirstOrDefault(x => x.ProductId == productOrderDto.ProductId);
            product.Quantity = product.Quantity - productOrderDto.Amount;
            await context.SaveChangesAsync();
            return mapper.Map<ProductOrderDto>(productOrder);


        }

        public async Task UpdateProductOrder(Guid productId, Guid orderId, ProductOrderUpdateDto productOrderDto)
        {
            var productOrder = await context.ProductOrderTable.FindAsync(orderId, productId);
            if (productOrder == null)
            {
                throw new ArgumentException("ProductOrder not found");
            }
            var oldAmount = productOrder.Amount;
            var newAmount = productOrderDto.Amount;
            var product = context.ProductTable.FirstOrDefault(x => x.ProductId == orderId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }
            product.Quantity = product.Quantity + oldAmount - newAmount;
            mapper.Map(productOrderDto, productOrder);
            await context.SaveChangesAsync();
        }

        public async Task DeleteProductOrder(Guid productId, Guid orderId)
        {
            var productOrder = await context.ProductOrderTable
                    .FirstOrDefaultAsync(op => op.OrderId == orderId && op.ProductId == productId);
            if (productOrder == null)
            {
                throw new ArgumentException("ProductOrder not found");
            }
            var product = context.ProductTable.FirstOrDefault(x => x.ProductId == productId);
            product.Quantity = product.Quantity + productOrder.Amount;
            context.ProductOrderTable.Remove(productOrder);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductOrderDto>> GetAllProductOrders(int page, int pageSize, string sortBy, string sortOrder)
        {
            var query = context.ProductOrderTable.AsQueryable();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
            {
                switch (sortBy.ToLower())
                {
                    case "amount":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(po => po.Amount) : query.OrderByDescending(po => po.Amount);
                        break;
                    case "orderid":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(po => po.OrderId) : query.OrderByDescending(po => po.OrderId);
                        break;
                    case "productid":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(po => po.ProductId) : query.OrderByDescending(po => po.ProductId);
                        break;
                    default:
                        break;
                }
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var productOrders = await query.ToListAsync();
            return mapper.Map<IEnumerable<ProductOrderDto>>(productOrders);
        }
    }

}