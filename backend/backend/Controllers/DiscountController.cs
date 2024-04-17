using AutoMapper;
using backend.Data.Repository;
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
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetAllDiscountes()
    {
        var discountes = await discountRepository.GetAllDiscounts();
        return Ok(discountes);
    }

    [Authorize(Roles = "Admin,User")]
    [HttpGet("{id}")]
    public async Task<ActionResult<DiscountDto>> GetDiscountById(Guid id)
    {
        var discount = await discountRepository.GetDiscountById(id);
        if (discount == null)
        {
            return NotFound();
        }

        return Ok(discount);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateDiscount(DiscountCreateDto discountDto)
    {
        var discountId = await discountRepository.CreateDiscount(discountDto);
        return Ok(discountId);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
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
