using AutoMapper;
using backend.Data.Repository;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/user")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;

    public UserController(IUserRepository userRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await userRepository.GetAllUsers();
        if (users == null || users.Count() == 0)
        {
            return NoContent();
        }
        return Ok(users);
    }

    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await userRepository.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [AllowAnonymous]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Guid>> CreateUser(UserCreateDto userDto)
    {
        if(userRepository.GetUserByEmail(userDto.Email) != null)
        {
            return Conflict("User with that email already exists");
        }
        var userId = await userRepository.CreateUser(userDto);
        return Ok(userId);
    }

    [Authorize(Roles = "User")]
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateUser(Guid id, UserUpdateDto userDto)
    {
        var oldUser = userRepository.GetUserById(id);
        if (oldUser == null)
        {
            return NotFound();
        }
        try
        {
            await userRepository.UpdateUser(id, userDto);
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
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = userRepository.GetUserById(id);
        if (user == null)
        {
            return NotFound();
        }
        try
        {
            await userRepository.DeleteUser(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("current-user")]
    public IActionResult GetCurrentUser()
    {
        var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);

 
        var userRole = HttpContext.User.FindFirstValue(ClaimTypes.Role);


        return Ok(new { Email = userEmail, Role = userRole });
    }
}