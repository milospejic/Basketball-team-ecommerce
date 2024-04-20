using AutoMapper;
using backend.Data.Context;
using backend.Models.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace backend.Data.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext context;
        private readonly IMapper mapper;
        private readonly static int iterations = 1000;

        public UserRepository(MyDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            var users = await context.UserTable.ToListAsync();
            foreach (var user in users)
            {
                var address = context.AddressTable.FirstOrDefault(f => f.AddressId == user.AddressId);
                if (address != null)
                {
                    user.Address = address;
                }
            }
            return mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto> GetUserById(Guid userId)
        {
            var user = await context.UserTable.FindAsync(userId);
            if (user != null)
            {
                var address = context.AddressTable.FirstOrDefault(f => f.AddressId == user.AddressId);
                if (address != null)
                {
                    user.Address = address;
                }
            }
            return mapper.Map<UserDto>(user);
        }

        public async Task<Guid> CreateUser(UserCreateDto userDto)
        {
            var (hashedPassword, salt) = HashPassword(userDto.Password);
            var user = mapper.Map<User>(userDto);
            user.Salt = salt;
            user.Password = hashedPassword;
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

        public bool UserWithCredentialsExists(string email, string password)
        {
            User user = context.UserTable.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return false;
            }

            if (VerifyPassword(password, user.Password, user.Salt))
            {
                return true;
            }
            return false;
        }


        private Tuple<string, string> HashPassword(string password)
        {
            var sBytes = new byte[password.Length];
            new RNGCryptoServiceProvider().GetNonZeroBytes(sBytes);
            var salt = Convert.ToBase64String(sBytes);

            var derivedBytes = new Rfc2898DeriveBytes(password, sBytes, iterations);

            return new Tuple<string, string>
            (
                Convert.ToBase64String(derivedBytes.GetBytes(256)),
                salt
            );
        }

        public bool VerifyPassword(string password, string savedHash, string savedSalt)
        {
            var saltBytes = Convert.FromBase64String(savedSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, iterations);
            if (Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256)) == savedHash)
            {
                return true;
            }
            return false;
        }

        public User GetCurrentUser(ClaimsPrincipal user)
        {
            var userEmail = user.FindFirstValue(ClaimTypes.Email);

            return context.UserTable.FirstOrDefault(u => u.Email == userEmail);
        }

        public User GetUserByEmail(string email)
        {
            var user = context.UserTable.FirstOrDefault(u => u.Email == email);
            if(user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsers(int page, int pageSize, string sortBy, string sortOrder)
        {
            var query = context.UserTable.AsQueryable();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
            {
                switch (sortBy.ToLower())
                {
                    case "name":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(u => u.Name) : query.OrderByDescending(p => p.Name);
                        break;
                    case "surname":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(u => u.Surname) : query.OrderByDescending(p => p.Surname);
                        break;
                    case "email":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(u => u.Email) : query.OrderByDescending(p => p.Email);
                        break;
                    case "dateofbirth":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(u => u.DateOfBirth) : query.OrderByDescending(p => p.DateOfBirth);
                        break;                    
                    default:
                        break;
                }
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var users = await query.ToListAsync();
            return mapper.Map<IEnumerable<UserDto>>(users);
        }
    }

}