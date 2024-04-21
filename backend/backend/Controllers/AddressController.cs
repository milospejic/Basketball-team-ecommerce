using AutoMapper;
using backend.Data.Repository;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("api/address")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly IAddressRepository addressRepository;
    private readonly IMapper mapper;

    public AddressController(IAddressRepository addressRepository, IMapper mapper)
    {
        this.addressRepository = addressRepository;
        this.mapper = mapper;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetAllAddresses(int page = 1, int pageSize = 10, string sortBy = "", string sortOrder = "")
    {
        var addresses = await addressRepository.GetAllAddresss(page, pageSize, sortBy, sortOrder);
        if (addresses == null || addresses.Count() == 0)
        {
            return NoContent();
        }
        return Ok(addresses);
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "User,Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<AddressDto>> GetAddressById(Guid id)
    {
        var address = await addressRepository.GetAddressById(id);
        if (address == null)
        {
            return NotFound("There is no address with id: "+ id);
        }

        return Ok(address);
    }

    [Authorize(Roles = "User")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AddressDto>> CreateAddress(AddressCreateDto addressDto)
    {
        var address = await addressRepository.CreateAddress(addressDto);
        return Ok(address);
    }

    [Authorize(Roles = "User")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAddress(Guid id, AddressUpdateDto addressDto)
    {
       
        try
        {
            await addressRepository.UpdateAddress(id, addressDto);
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
    public async Task<IActionResult> DeleteAddress(Guid id)
    {
        try
        {
            await addressRepository.DeleteAddress(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
