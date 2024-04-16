using Microsoft.AspNetCore.Identity;

namespace backend.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? AddressId { get; set; }
        public Address Address { get; set; }

        public string Salt { get; set; }
    }
}
