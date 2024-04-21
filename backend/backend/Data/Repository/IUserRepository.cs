using backend.Models;
using backend.Models.Dtos;
using System.Security.Claims;

namespace backend.Data.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(Guid userId);
        Task<UserDto> CreateUser(UserCreateDto userDto);
        Task UpdateUser(Guid userId, UserUpdateDto userDto);
        Task DeleteUser(Guid userId);
        bool UserWithCredentialsExists(string email, string password);
        User GetCurrentUser(ClaimsPrincipal user);

        User GetUserByEmail(string email);
        Task<IEnumerable<UserDto>> GetAllUsers(int page, int pageSize, string sortBy, string sortOrder);

    }
}