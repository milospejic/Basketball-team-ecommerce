namespace backend.Models
{
    public class Address
    {
        public Guid AddressId { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
    }
}
