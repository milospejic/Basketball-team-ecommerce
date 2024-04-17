using AutoMapper;
using backend.Data.Repository;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/product")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductRepository productRepository;
    private readonly IMapper mapper;

    public ProductController(IProductRepository productRepository, IMapper mapper)
    {
        this.productRepository = productRepository;
        this.mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await productRepository.GetAllProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        var product = await productRepository.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }


    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProduct(ProductCreateDto productDto)
    {
        var productId = await productRepository.CreateProduct(productDto);
        return Ok(productId);
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, ProductUpdateDto productDto)
    {
        try
        {
            await productRepository.UpdateProduct(id, productDto);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            await productRepository.DeleteProduct(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
