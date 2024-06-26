﻿using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IReviewRepository
    {
        Task<IEnumerable<ReviewDto>> GetAllReviews();
        Task<ReviewDto> GetReviewById(Guid reviewId);
        Task<ReviewDto> CreateReview(ReviewCreateDto reviewDto, Guid userId);
        Task UpdateReview(Guid reviewId, ReviewUpdateDto reviewDto);
        Task DeleteReview(Guid reviewId);
        Task<IEnumerable<ReviewDto>> GetReviewsByProductId(Guid productId);
        Task<IEnumerable<ReviewDto>> GetReviewsByUserId(Guid productId);
        Task<IEnumerable<ReviewDto>> GetAllReviews(int page, int pageSize, string sortBy, string sortOrder);
    }
}
