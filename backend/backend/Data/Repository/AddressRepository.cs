using AutoMapper;
using backend.Data.Context;
using backend.Models.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data.Repository
{
    public class AddressRepository : IAddressRepository
    {
        private readonly MyDbContext context;
        private readonly IMapper mapper;

        public AddressRepository(MyDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<IEnumerable<AddressDto>> GetAllAddresss()
        {
            var addresses = await context.AddressTable.ToListAsync();
            return mapper.Map<IEnumerable<AddressDto>>(addresses);
        }

        public async Task<AddressDto> GetAddressById(Guid addressId)
        {
            var address = await context.AddressTable.FindAsync(addressId);
            return mapper.Map<AddressDto>(address);
        }

        public async Task<Guid> CreateAddress(AddressCreateDto addressDto)
        {
            var address = mapper.Map<Address>(addressDto);
            context.AddressTable.Add(address);
            await context.SaveChangesAsync();
            return address.AddressId;
        }

        public async Task UpdateAddress(Guid addressId, AddressUpdateDto addressDto)
        {
            var address = await context.AddressTable.FindAsync(addressId);
            if (address == null)
            {
                throw new ArgumentException("Address not found");
            }

            mapper.Map(addressDto, address);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAddress(Guid addressId)
        {
            var address = await context.AddressTable.FindAsync(addressId);
            if (address == null)
            {
                throw new ArgumentException("Address not found");
            }

            context.AddressTable.Remove(address);
            await context.SaveChangesAsync();
        }

    }
}
