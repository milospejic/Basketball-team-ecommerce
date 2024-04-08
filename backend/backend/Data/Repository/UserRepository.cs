using AutoMapper;
using backend.Data.Context;
using backend.Models.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext context;
        private readonly IMapper mapper;

        public UserRepository(MyDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await context.UserTable.ToListAsync();
            return mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserById(Guid userId)
        {
            var user = await context.UserTable.FindAsync(userId);
            return mapper.Map<UserDto>(user);
        }

        public async Task<Guid> CreateUser(UserCreateDto userDto)
        {
            var user = mapper.Map<User>(userDto);
            context.UserTable.Add(user);
            await context.SaveChangesAsync();
            return user.UserId;
        }

        public async Task UpdateUser(Guid userId, UserUpdateDto userDto)
        {
            var user = await context.UserTable.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            mapper.Map(userDto, user);
            await context.SaveChangesAsync();
        }

        public async Task DeleteUser(Guid userId)
        {
            var user = await context.UserTable.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            context.UserTable.Remove(user);
            await context.SaveChangesAsync();
        }
    }

}
