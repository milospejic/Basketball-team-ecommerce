using backend.Auth;
using backend.Models;
using backend.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth auth;

        public AuthController(IAuth auth)
        {
            this.auth = auth;
        }

        [HttpPost("authenticate")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Authenticate(LoginDto loginDto)
        {
            if (auth.AuthenticateUser(loginDto))
            {
                var tokenString = auth.GenerateJwt(loginDto);
                return Ok(new { token = tokenString });
            }
            return Unauthorized();
        }

    }
}
