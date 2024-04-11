using AutoMapper;
using backend.Data.Repository;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

[Route("api/review")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository reviewRepository;
    private readonly IMapper mapper;

    public ReviewController(IReviewRepository reviewRepository, IMapper mapper)
    {
        this.reviewRepository = reviewRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviewes()
    {
        var reviewes = await reviewRepository.GetAllReviews();
        return Ok(reviewes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetReviewById(Guid id)
    {
        var review = await reviewRepository.GetReviewById(id);
        if (review == null)
        {
            return NotFound();
        }

        return Ok(review);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateReview(ReviewCreateDto reviewDto)
    {
        var reviewId = await reviewRepository.CreateReview(reviewDto);
        return Ok(reviewId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(Guid id, ReviewUpdateDto reviewDto)
    {
        try
        {
            await reviewRepository.UpdateReview(id, reviewDto);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        try
        {
            await reviewRepository.DeleteReview(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
