using AutoMapper;
using backend.Data.Repository;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/productOrder")]
[ApiController]
public class ProductOrderController : ControllerBase
{
    private readonly IProductOrderRepository productOrderRepository;
    private readonly IMapper mapper;

    public ProductOrderController(IProductOrderRepository productOrderRepository, IMapper mapper)
    {
        this.productOrderRepository = productOrderRepository;
        this.mapper = mapper;
    }

    [Authorize(Roles = "User")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<ProductOrderDto>>> GetAllProductOrders(int page = 1, int pageSize = 10, string sortBy = "", string sortOrder = "")
    {
        var productOrders = await productOrderRepository.GetAllProductOrders(page, pageSize, sortBy, sortOrder);
        if (productOrders == null || productOrders.Count() == 0)
        {
            return NoContent();
        }
        return Ok(productOrders);
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "User")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductOrderDto>> GetProductOrderById(Guid id, Guid id2)
    {
        var productOrder = await productOrderRepository.GetProductOrderById(id, id2);
        if (productOrder == null)
        {
            return NotFound();
        }

        return Ok(productOrder);
    }


    [Authorize(Roles = "User")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateProductOrder(ProductOrderCreateDto productOrderDto)
    {
        try { 
        var productOrderId = await productOrderRepository.CreateProductOrder(productOrderDto);
        return Ok(productOrderId);
        }
         catch (ArgumentException ex)
         {
             return BadRequest(ex.Message);
         }
    }


    [Authorize(Roles = "User")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
     public async Task<IActionResult> UpdateProductOrder(Guid id, Guid id2, ProductOrderUpdateDto productOrderDto)
     {
        /*
        var oldProductOrder = productOrderRepository.GetProductOrderById(id,id2);
        if (oldProductOrder == null)
        {
            return NotFound();
        }*/
         try
         {
             await productOrderRepository.UpdateProductOrder(id,id2, productOrderDto);
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
    public async Task<IActionResult> DeleteProductOrder(Guid id, Guid id2)
    {
        /*var productOrder = productOrderRepository.GetProductOrderById(id, id2);
        if (productOrder == null)
        {
            return NotFound();
        }*/
        try
        {
            await productOrderRepository.DeleteProductOrder(id, id2);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}