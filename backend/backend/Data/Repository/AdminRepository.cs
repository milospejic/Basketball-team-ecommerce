using AutoMapper;
using backend.Models.Dtos;
using backend.Models;
using backend.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Repository
{
    public class AdminRepository : IAdminRepository
    {
        private readonly MyDbContext context;
        private readonly IMapper mapper;

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

        public async Task<Guid> CreateAdmin(AdminCreateDto adminDto)
        {
            var admin = mapper.Map<Admin>(adminDto);
            context.AdminTable.Add(admin);
            await context.SaveChangesAsync();
            return admin.AdminId;
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

    }
}
