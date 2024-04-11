using AutoMapper;
using backend.Data.Repository;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AddressDto>>> GetAllAddresses()
    {
        var addresses = await addressRepository.GetAllAddresss();
        return Ok(addresses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AddressDto>> GetAddressById(Guid id)
    {
        var address = await addressRepository.GetAddressById(id);
        if (address == null)
        {
            return NotFound();
        }

        return Ok(address);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAddress(AddressCreateDto addressDto)
    {
        var addressId = await addressRepository.CreateAddress(addressDto);
        return Ok(addressId);
    }

    [HttpPut("{id}")]
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

    [HttpDelete("{id}")]
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
