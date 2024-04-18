using AutoMapper;
using backend.Data.Repository;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminRepository adminRepository;
    private readonly IMapper mapper;

    public AdminController(IAdminRepository adminRepository, IMapper mapper)
    {
        this.adminRepository = adminRepository;
       this.mapper = mapper;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<AdminDto>>> GetAllAdmins()
    {
        var admins = await adminRepository.GetAllAdmins();
        if (admins == null || admins.Count() == 0)
        {
            return NoContent();
        }
        return Ok(admins);
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<AdminDto>> GetAdminById(Guid id)
    {
        var admin = await adminRepository.GetAdminById(id);
        if (admin == null)
        {
            return NotFound();
        }

        return Ok(admin);
    }



    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateAdmin(AdminCreateDto adminDto)
    {
        var adminId = await adminRepository.CreateAdmin(adminDto);
        return Ok(adminId);
    }


    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAdmin(Guid id, AdminUpdateDto adminDto)
    {
        var oldAdmin = adminRepository.GetAdminById(id);
        if (oldAdmin == null)
        {
            return NotFound();
        }
        try
        {
            await adminRepository.UpdateAdmin(id, adminDto);
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
    public async Task<IActionResult> DeleteAdmin(Guid id)
    {

        var admin = adminRepository.GetAdminById(id);
        if (admin == null)
        {
            return NotFound();
        }
        try
        {
            await adminRepository.DeleteAdmin(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
