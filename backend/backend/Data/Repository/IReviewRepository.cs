using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IReviewRepository
    {
        Task<IEnumerable<ReviewDto>> GetAllReviews();
        Task<ReviewDto> GetReviewById(Guid reviewId);
        Task<Guid> CreateReview(ReviewCreateDto reviewDto);
        Task UpdateReview(Guid reviewId, ReviewUpdateDto reviewDto);
        Task DeleteReview(Guid reviewId);
    }
}
