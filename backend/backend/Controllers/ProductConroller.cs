using AutoMapper;
using backend.Data.Repository;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/product")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductRepository productRepository;
    private readonly IMapper mapper;
    private readonly IAdminRepository adminRepository;
    public ProductController(IProductRepository productRepository, IMapper mapper, IAdminRepository adminRepository)
    {
        this.productRepository = productRepository;
        this.mapper = mapper;
        this.adminRepository = adminRepository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts(int page = 1, int pageSize = 10, string sortBy = "", string sortOrder = "")
    {
        var products = await productRepository.GetAllProducts(page, pageSize, sortBy, sortOrder);
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
            return NotFound("There is no product with id: " + id);
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
        var admin = adminRepository.GetAdminByEmail(User.FindFirst(ClaimTypes.Email)?.Value);
        var product = await productRepository.CreateProduct(productDto, admin.AdminId);
        return Ok(product);
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    [Authorize(Roles = "Admin")]
    [HttpGet("admin/{adminId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllProductsForAdmin(Guid adminId)
    {
        var products = await productRepository.GetProductsByAdminId(adminId);
        if (products == null || products.Count() == 0)
        {
            return NoContent();
        }
        return Ok(products);
    }

    [AllowAnonymous]
    [HttpGet("name")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllProductsByName(string productName)
    {
        var products = await productRepository.GetProductsByName(productName);
        if (products == null || products.Count() == 0)
        {
            return NoContent();
        }
        return Ok(products);
    }

    [AllowAnonymous]
    [HttpGet("category")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllProductsByCategory(string category)
    {
        var products = await productRepository.GetProductsByCategory(category);
        if (products == null || products.Count() == 0)
        {
            return NoContent();
        }
        return Ok(products);
    }

    [AllowAnonymous]
    [HttpGet("brand")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllProductsByBrand(string brand)
    {
        var products = await productRepository.GetProductsByBrand(brand);
        if (products == null || products.Count() == 0)
        {
            return NoContent();
        }
        return Ok(products);
    }

    [AllowAnonymous]
    [HttpGet("size")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllProductsBySize(string size)
    {
        var products = await productRepository.GetProductsBySize(size);
        if (products == null || products.Count() == 0)
        {
            return NoContent();
        }
        return Ok(products);
    }
}
