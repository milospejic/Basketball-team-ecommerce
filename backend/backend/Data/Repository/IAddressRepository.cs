using backend.Models.Dtos;

namespace backend.Data.Repository
{
    public interface IAddressRepository
    {
        Task<IEnumerable<AddressDto>> GetAllAddresss();
        Task<AddressDto> GetAddressById(Guid addressId);
        Task<Guid> CreateAddress(AddressCreateDto addressDto);
        Task UpdateAddress(Guid addressId, AddressUpdateDto addressDto);
        Task DeleteAddress(Guid addressId);
        Task<IEnumerable<AddressDto>> GetAllAddresss(int page, int pageSize, string sortBy, string sortOrder);
    }
}
