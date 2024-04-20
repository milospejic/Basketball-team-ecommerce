using AutoMapper;
using backend.Data.Repository;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/review")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepository reviewRepository;
    private readonly IMapper mapper;
    private readonly IUserRepository userRepository;
    public ReviewController(IReviewRepository reviewRepository, IMapper mapper, IUserRepository userRepository)
    {
        this.reviewRepository = reviewRepository;
        this.mapper = mapper;
        this.userRepository = userRepository;
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetAllReviews(int page = 1, int pageSize = 10, string sortBy = "", string sortOrder = "")
    {
        var reviews = await reviewRepository.GetAllReviews(page, pageSize, sortBy, sortOrder);
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
        var user = userRepository.GetUserByEmail(User.FindFirst(ClaimTypes.Email)?.Value);
        var reviewId = await reviewRepository.CreateReview(reviewDto,user.UserId);
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

    [AllowAnonymous]
    [HttpGet("product/{productId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllReviewsForProduct(Guid productId)
    {
        var reviews = await reviewRepository.GetReviewsByProductId(productId);
        if (reviews == null || reviews.Count() == 0)
        {
            return NoContent();
        }
        return Ok(reviews);
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("user/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllReviewsForUser(Guid userId)
    {
        var reviews = await reviewRepository.GetReviewsByUserId(userId);
        if (reviews == null || reviews.Count() == 0)
        {
            return NoContent();
        }
        return Ok(reviews);
    }
}
