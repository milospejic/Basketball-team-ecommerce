using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetAllUsers();
        Task<UserDto> GetUserById(Guid userId);
        Task<Guid> CreateUser(UserCreateDto userDto);
        Task UpdateUser(Guid userId, UserUpdateDto userDto);
        Task DeleteUser(Guid userId);
    }
}
