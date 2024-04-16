using backend.Models.Dtos;

namespace backend.Auth
{
    public interface IAuth
    {
        public bool AuthenticateUser(LoginDto loginDto);

        public bool IsAdmin(LoginDto loginDto);

        public string GenerateJwt(LoginDto loginDto);
    }
}
