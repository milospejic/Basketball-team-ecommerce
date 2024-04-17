using AutoMapper;
using backend.Data.Repository;
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

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductOrderDto>>> GetAllProductOrderes()
    {
        var productOrderes = await productOrderRepository.GetAllProductOrders();
        return Ok(productOrderes);
    }

    [Authorize(Roles = "Admin")]
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


    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProductOrder(ProductOrderCreateDto productOrderDto)
    {
        var productOrderId = await productOrderRepository.CreateProductOrder(productOrderDto);
        return Ok(productOrderId);
    }

    /* [HttpPut("{id}")]
     public async Task<IActionResult> UpdateProductOrder(Guid id, Guid id2, ProductOrderUpdateDto productOrderDto)
     {
         try
         {
             await productOrderRepository.UpdateProductOrder(id,id2, productOrderDto);
             return Ok();
         }
         catch (ArgumentException ex)
         {
             return BadRequest(ex.Message);
         }
     }*/


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProductOrder(Guid id, Guid id2)
    {
        try
        {
            await productOrderRepository.DeleteProductOrder(id, id2    );
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
