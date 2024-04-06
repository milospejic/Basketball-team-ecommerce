namespace backend.Models.Dtos
{
    public class AdminDto
    {
        public Guid AdminId { get; set; }
        public string AdminName { get; set; }
        public string AdminSurname { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPhoneNumber { get; set; }
    }
}
