using AutoMapper;
using backend.Models.Dtos;
using backend.Models;
using backend.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace backend.Data.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly MyDbContext context;
        private readonly IMapper mapper;
        private readonly static int iterations = 1000;

        public AdminRepository(MyDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<AdminDto>> GetAllAdmins()
        {
            var admins = await context.AdminTable.ToListAsync();
            return mapper.Map<IEnumerable<AdminDto>>(admins);
        }

        public async Task<AdminDto> GetAdminById(Guid id)
        {
            var admin = await context.AdminTable.FindAsync(id);
            return mapper.Map<AdminDto>(admin);
        }

        public async Task<AdminDto> CreateAdmin(AdminCreateDto adminDto)
        {
            var (hashedPassword, salt) = HashPassword(adminDto.AdminPassword);
            var admin = mapper.Map<Admin>(adminDto);
            admin.AdminSalt = salt;
            admin.AdminPassword = hashedPassword;
            context.AdminTable.Add(admin);
            await context.SaveChangesAsync();
            return mapper.Map<AdminDto>(admin);
        }

        public async Task UpdateAdmin(Guid id, AdminUpdateDto adminDto)
        {
            var admin = await context.AdminTable.FindAsync(id);
            if (admin == null)
            {
                throw new ArgumentException("Admin not found");
            }

            mapper.Map(adminDto, admin);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAdmin(Guid id)
        {
            var admin = await context.AdminTable.FindAsync(id);
            if (admin == null)
            {
                throw new ArgumentException("Admin not found");
            }

            context.AdminTable.Remove(admin);
            await context.SaveChangesAsync();
        }

        public bool AdminWithCredentialsExists(string email, string password)
        {
            Admin admin = context.AdminTable.FirstOrDefault(a => a.AdminEmail == email);

            if (admin == null)
            {
                return false;
            }

            if (VerifyPassword(password, admin.AdminPassword, admin.AdminSalt))
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

        public Admin GetAdminByEmail(string email)
        {
            var admin = context.AdminTable.FirstOrDefault(a => a.AdminEmail == email);
            if (admin == null)
            {
                return null;
            }
            return admin;
        }

        public async Task<IEnumerable<AdminDto>> GetAllAdmins(int page, int pageSize, string sortBy, string sortOrder)
        {
            var query = context.AdminTable.AsQueryable();

            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
            {
                switch (sortBy.ToLower())
                {
                    case "adminname":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.AdminName) : query.OrderByDescending(a => a.AdminName);
                        break;
                    case "adminsurname":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.AdminSurname) : query.OrderByDescending(a => a.AdminSurname);
                        break;
                    case "adminemail":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.AdminEmail) : query.OrderByDescending(a => a.AdminEmail);
                        break;
                    default:
                        break;
                }
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var admins = await query.ToListAsync();
            return mapper.Map<IEnumerable<AdminDto>>(admins);
        }
    }
}
