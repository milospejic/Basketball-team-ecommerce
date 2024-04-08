using AutoMapper;
using backend.Data.Context;
using backend.Models.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly MyDbContext context;
        private readonly IMapper mapper;

        public ReviewRepository(MyDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviews()
        {
            var reviews = await context.ReviewTable.ToListAsync();
            return mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }

        public async Task<ReviewDto> GetReviewById(Guid reviewId)
        {
            var review = await context.ReviewTable.FindAsync(reviewId);
            return mapper.Map<ReviewDto>(review);
        }

        public async Task<Guid> CreateReview(ReviewCreateDto reviewDto)
        {
            var review = mapper.Map<Review>(reviewDto);
            context.ReviewTable.Add(review);
            await context.SaveChangesAsync();
            return review.ReviewId;
        }

        public async Task UpdateReview(Guid reviewId, ReviewUpdateDto reviewDto)
        {
            var review = await context.ReviewTable.FindAsync(reviewId);
            if (review == null)
            {
                throw new ArgumentException("Review not found");
            }

            mapper.Map(reviewDto, review);
            await context.SaveChangesAsync();
        }

        public async Task DeleteReview(Guid reviewId)
        {
            var review = await context.ReviewTable.FindAsync(reviewId);
            if (review == null)
            {
                throw new ArgumentException("Review not found");
            }

            context.ReviewTable.Remove(review);
            await context.SaveChangesAsync();
        }
    }

}
