using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<DiscountDto>> GetAllDiscounts();
        Task<DiscountDto> GetDiscountById(Guid discountId);
        Task<Guid> CreateDiscount(DiscountCreateDto discountDto);
        Task UpdateDiscount(Guid discountId, DiscountUpdateDto discountDto);
        Task DeleteDiscount(Guid discountId);
        Task<IEnumerable<DiscountDto>> GetAllDiscounts(int page, int pageSize, string sortBy, string sortOrder);

    }
}
