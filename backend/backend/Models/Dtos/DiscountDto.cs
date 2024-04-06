namespace backend.Models.Dtos
{
    public class DiscountDto
    {
        public Guid DiscountId { get; set; }
        public string DiscountType { get; set; }
        public string Percentage { get; set; }
    }
}
