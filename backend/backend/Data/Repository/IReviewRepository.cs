using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IReviewRepository
    {
        Task<IEnumerable<ReviewDto>> GetAllReviews();
        Task<ReviewDto> GetReviewById(Guid reviewId);
        Task<Guid> CreateReview(ReviewCreateDto reviewDto, Guid userId);
        Task UpdateReview(Guid reviewId, ReviewUpdateDto reviewDto);
        Task DeleteReview(Guid reviewId);
        Task<IEnumerable<ReviewDto>> GetReviewsByProductId(Guid productId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserId(Guid productId);
    }
}
