using AutoMapper;
using backend.Data.Context;
using backend.Models.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Repository
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly MyDbContext context;
        private readonly IMapper mapper;

        public DiscountRepository(MyDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<DiscountDto>> GetAllDiscounts()
        {
            var discounts = await context.DiscountTable.ToListAsync();
            return mapper.Map<IEnumerable<DiscountDto>>(discounts);
        }

        public async Task<DiscountDto> GetDiscountById(Guid discountId)
        {
            var discount = await context.DiscountTable.FindAsync(discountId);
            return mapper.Map<DiscountDto>(discount);
        }

        public async Task<Guid> CreateDiscount(DiscountCreateDto discountDto)
        {
            var discount = mapper.Map<Discount>(discountDto);
            context.DiscountTable.Add(discount);
            await context.SaveChangesAsync();
            return discount.DiscountId;
        }

        public async Task UpdateDiscount(Guid discountId, DiscountUpdateDto discountDto)
        {
            var discount = await context.DiscountTable.FindAsync(discountId);
            if (discount == null)
            {
                throw new ArgumentException("Discount not found");
            }

            mapper.Map(discountDto, discount);
            await context.SaveChangesAsync();
        }

        public async Task DeleteDiscount(Guid discountId)
        {
            var discount = await context.DiscountTable.FindAsync(discountId);
            if (discount == null)
            {
                throw new ArgumentException("Discount not found");
            }

            context.DiscountTable.Remove(discount);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DiscountDto>> GetAllDiscounts(int page, int pageSize, string sortBy, string sortOrder)
        {
            var query = context.DiscountTable.AsQueryable();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
            {
                switch (sortBy.ToLower())
                {
                   
                    case "discounttype":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(d => d.DiscountType) : query.OrderByDescending(d => d.DiscountType);
                        break;
                    case "percentage":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(d => d.Percentage) : query.OrderByDescending(d => d.Percentage);
                        break;
                    default:
                        break;
                }
            }

            // Apply pagination
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var discounts = await query.ToListAsync();
            return mapper.Map<IEnumerable<DiscountDto>>(discounts);
        }
    }

}
