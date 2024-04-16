namespace backend.Models
{
    public class Admin
    {
        public Guid AdminId { get; set; }
        public string AdminName { get; set; }
        public string AdminSurname { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPassword { get; set; }
        public string AdminPhoneNumber { get; set; }
        public string AdminSalt { get; set; }
    }
}
