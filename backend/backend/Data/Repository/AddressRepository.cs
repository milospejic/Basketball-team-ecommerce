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

        public async Task<IEnumerable<AddressDto>> GetAllAddresss(int page, int pageSize, string sortBy, string sortOrder)
        {
            var query = context.AddressTable.AsQueryable();


            if (!string.IsNullOrEmpty(sortBy) && !string.IsNullOrEmpty(sortOrder))
            {
                switch (sortBy.ToLower())
                {
                    case "street":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.Street) : query.OrderByDescending(a => a.Street);
                        break;
                    case "streetnumber":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.StreetNumber) : query.OrderByDescending(a => a.StreetNumber);
                        break;
                    case "town":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.Town) : query.OrderByDescending(a => a.Town);
                        break;
                    case "country":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(a => a.Country) : query.OrderByDescending(a => a.Country);
                        break;
                    default:
                        break;
                }
            }

            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            var addresses = await query.ToListAsync();
            return mapper.Map<IEnumerable<AddressDto>>(addresses);
        }
    }
}
