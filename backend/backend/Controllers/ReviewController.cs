using AutoMapper;
using backend.Data.Repository;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviews()
    {
        var reviews = await reviewRepository.GetAllReviews();
        if (reviews == null || reviews.Count() == 0)
        {
            return NoContent();
        }
        return Ok(reviews);
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "User,Admin")]
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

    [Authorize(Roles = "User")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateReview(ReviewCreateDto reviewDto)
    {
        var reviewId = await reviewRepository.CreateReview(reviewDto);
        return Ok(reviewId);
    }

    [Authorize(Roles = "User")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateReview(Guid id, ReviewUpdateDto reviewDto)
    {
        var oldReview = reviewRepository.GetReviewById(id);
        if (oldReview == null)
        {
            return NotFound();
        }
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

    [Authorize(Roles = "User")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var review = reviewRepository.GetReviewById(id);
        if (review == null)
        {
            return NotFound();
        }
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
