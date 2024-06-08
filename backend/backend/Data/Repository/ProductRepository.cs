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

        public async Task<ProductDto> CreateProduct(ProductCreateDto productDto, Guid adminId)
        {
            var product = mapper.Map<Product>(productDto);
            product.AdminId = adminId;
            context.ProductTable.Add(product);
            await context.SaveChangesAsync();
            return mapper.Map<ProductDto>(product);
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

        public async Task<IEnumerable<ProductDto>> GetProductsByAdminId(Guid adminId)
        {
            var products = await context.ProductTable.ToListAsync();
            List<Product> returnProducts = new List<Product>();
            foreach (var product in products)
            {
                if (product.AdminId == adminId)
                {
                    returnProducts.Add(product);
                }
            }
            return mapper.Map<IEnumerable<ProductDto>>(returnProducts);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByName(string productName)
        {
            var products = await context.ProductTable.ToListAsync();
            List<Product> returnProducts = new List<Product>();
            foreach (var product in products)
            {
                if (product.ProductName == productName)
                {
                    returnProducts.Add(product);
                }
            }
            return mapper.Map<IEnumerable<ProductDto>>(returnProducts);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByBrand(string brand)
        {
            var products = await context.ProductTable.ToListAsync();
            List<Product> returnProducts = new List<Product>();
            foreach (var product in products)
            {
                if (product.Brand == brand)
                {
                    returnProducts.Add(product);
                }
            }
            return mapper.Map<IEnumerable<ProductDto>>(returnProducts);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategory(string category)
        {
            var products = await context.ProductTable.ToListAsync();
            List<Product> returnProducts = new List<Product>();
            foreach (var product in products)
            {
                if (product.Category == category)
                {
                    returnProducts.Add(product);
                }
            }
            return mapper.Map<IEnumerable<ProductDto>>(returnProducts);
        }

        public async Task<IEnumerable<ProductDto>> SearchProducts(string query, int page, int pageSize)
        {
            var products = await context.ProductTable
                .Where(p => p.ProductName.Contains(query) || p.Description.Contains(query))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return mapper.Map<IEnumerable<ProductDto>>(products);
        }
        public async Task<IEnumerable<ProductDto>> GetProductsBySize(string size)
        {
            var products = await context.ProductTable.ToListAsync();
            List<Product> returnProducts = new List<Product>();
            foreach (var product in products)
            {
                if (product.Size == size)
                {
                    returnProducts.Add(product);
                }
            }
            return mapper.Map<IEnumerable<ProductDto>>(returnProducts);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProducts(int page, int pageSize, string sortBy, string sortOrder)
        {
            var query = context.ProductTable.AsQueryable();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
            {
                switch (sortBy.ToLower())
                {
                    case "productname":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.ProductName) : query.OrderByDescending(p => p.ProductName);
                        break;
                    case "price":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                        break;
                    case "brand":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand);
                        break;
                    case "category":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.Category) : query.OrderByDescending(p => p.Category);
                        break;
                    case "size":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.Size) : query.OrderByDescending(p => p.Size);
                        break;
                    case "totalrating":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.TotalRating) : query.OrderByDescending(p => p.TotalRating);
                        break;
                    case "numofreviews":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.NumOfReviews) : query.OrderByDescending(p => p.NumOfReviews);
                        break;
                    case "adminid":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.AdminId) : query.OrderByDescending(p => p.AdminId);
                        break;
                    case "discountid":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.DiscountId) : query.OrderByDescending(p => p.DiscountId);
                        break;
                    default:
                        break;
                }
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var products = await query.ToListAsync();
            return mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }

}
