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

            var product = await context.ProductTable.FirstOrDefaultAsync(f => f.ProductId == review.ProductId);
            if (product == null)
            {
                throw new ArgumentException("Product not found");
            }

            double oldRating = review.Rating;
            double newRating = reviewDto.Rating;

            // Calculate the new total rating
            double numOfReviews = product.NumOfReviews;
            product.TotalRating = (product.TotalRating * numOfReviews - oldRating + newRating) / numOfReviews;

            // Update the review with the new data
            mapper.Map(reviewDto, review);

            // Save changes
            await context.SaveChangesAsync();
        }

        public async Task DeleteReview(Guid reviewId)
        {
            var review = await context.ReviewTable.FindAsync(reviewId);
            if (review == null)
            {
                throw new ArgumentException("Review not found");
            }

            review.IsDeleted = true;
            await context.SaveChangesAsync(); // Save changes to trigger the update in the database

            // Now, the trigger will run and update the ProductTable

            // Finally, remove the review from the database
            context.ReviewTable.Remove(review);
            await context.SaveChangesAsync();
        }
    }

}
