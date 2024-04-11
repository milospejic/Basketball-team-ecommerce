using AutoMapper;
using backend.Data.Repository;
using backend.Models.Dtos;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdminDto>>> GetAllAdmines()
    {
        var admines = await adminRepository.GetAllAdmins();
        return Ok(admines);
    }

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

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAdmin(AdminCreateDto adminDto)
    {
        var adminId = await adminRepository.CreateAdmin(adminDto);
        return Ok(adminId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAdmin(Guid id, AdminUpdateDto adminDto)
    {
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAdmin(Guid id)
    {
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
