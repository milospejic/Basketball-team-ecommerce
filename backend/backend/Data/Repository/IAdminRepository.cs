using backend.Models;
using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IAdminRepository
    {
        Task<IEnumerable<AdminDto>> GetAllAdmins();
        Task<AdminDto> GetAdminById(Guid adminId);
        Task<Guid> CreateAdmin(AdminCreateDto adminDto);
        Task UpdateAdmin(Guid adminId, AdminUpdateDto adminDto);
        Task DeleteAdmin(Guid adminId);
        bool AdminWithCredentialsExists(string email, string password);

        Admin GetAdminByEmail(string email);
    }
}
