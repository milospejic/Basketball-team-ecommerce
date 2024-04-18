using AutoMapper;
using backend.Data.Repository;
using backend.Models;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await productRepository.GetAllProducts();
        if (products == null || products.Count() == 0)
        {
            return NoContent();
        }
        return Ok(products);
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
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
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateProduct(ProductCreateDto productDto)
    {
        var productId = await productRepository.CreateProduct(productDto);
        return Ok(productId);
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateProduct(Guid id, ProductUpdateDto productDto)
    {
        var oldProduct = productRepository.GetProductById(id);
        if (oldProduct == null)
        {
            return NotFound();
        }
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var product = productRepository.GetProductById(id);
        if (product == null)
        {
            return NotFound();
        }
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
