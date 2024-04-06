namespace backend.Models.Dtos
{
    public class AddressCreateDto
    {
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
    }
}
