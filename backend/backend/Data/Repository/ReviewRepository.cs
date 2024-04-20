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

        public async Task<Guid> CreateReview(ReviewCreateDto reviewDto, Guid userId)
        {
            var review = mapper.Map<Review>(reviewDto);
            review.UserId = userId;
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

            
            double numOfReviews = product.NumOfReviews;
            product.TotalRating = (product.TotalRating * numOfReviews - oldRating + newRating) / numOfReviews;

            
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

            review.IsDeleted = true;
            await context.SaveChangesAsync(); 

           
            context.ReviewTable.Remove(review);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByProductId(Guid productId)
        {
            var reviews = await context.ReviewTable.ToListAsync();
            List<Review> returnReviews = new List<Review>();
            foreach (var review in reviews)
            {
                if (review.ProductId == productId)
                {
                    returnReviews.Add(review);
                }
            }
            return mapper.Map<IEnumerable<ReviewDto>>(returnReviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetReviewsByUserId(Guid userId)
        {
            var reviews = await context.ReviewTable.ToListAsync();
            List<Review> returnReviews = new List<Review>();
            foreach (var review in reviews)
            {
                if (review.UserId == userId)
                {
                    returnReviews.Add(review);
                }
            }
            return mapper.Map<IEnumerable<ReviewDto>>(returnReviews);
        }

        public async Task<IEnumerable<ReviewDto>> GetAllReviews(int page, int pageSize, string sortBy, string sortOrder)
        {
            var query = context.ReviewTable.AsQueryable();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
            {
                switch (sortBy.ToLower())
                {
                    case "rating":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(r => r.Rating) : query.OrderByDescending(r => r.Rating);
                        break;
                    case "userid":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(r => r.UserId) : query.OrderByDescending(r => r.UserId);
                        break;
                    case "productid":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(r => r.ProductId) : query.OrderByDescending(r => r.ProductId);
                        break;
                    default:
                        break;
                }
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var reviews = await query.ToListAsync();
            return mapper.Map<IEnumerable<ReviewDto>>(reviews);
        }
    }

}
