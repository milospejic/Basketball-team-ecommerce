using AutoMapper;
using backend.Data.Repository;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/discount")]
[ApiController]
public class DiscountController : ControllerBase
{
    private readonly IDiscountRepository discountRepository;
    private readonly IMapper mapper;

    public DiscountController(IDiscountRepository discountRepository, IMapper mapper)
    {
        this.discountRepository = discountRepository;
        this.mapper = mapper;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetAllDiscounts(int page = 1, int pageSize = 10, string sortBy = "", string sortOrder = "")
    {
        var discounts = await discountRepository.GetAllDiscounts(page, pageSize, sortBy, sortOrder);
        if (discounts == null || discounts.Count() == 0)
        {
            return NoContent();
        }
        return Ok(discounts);
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin,User")]
    [HttpGet("{id}")]
    public async Task<ActionResult<DiscountDto>> GetDiscountById(Guid id)
    {
        var discount = await discountRepository.GetDiscountById(id);
        if (discount == null)
        {
            return NotFound("There is no discount for id: " + id);
        }

        return Ok(discount);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateDiscount(DiscountCreateDto discountDto)
    {
        var discountId = await discountRepository.CreateDiscount(discountDto);
        return Ok(discountId);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateDiscount(Guid id, DiscountUpdateDto discountDto)
    {
        
        try
        {
            await discountRepository.UpdateDiscount(id, discountDto);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteDiscount(Guid id)
    {
       
        try
        {
            await discountRepository.DeleteDiscount(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
