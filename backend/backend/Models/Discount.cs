namespace backend.Models
{
    public class Discount
    {
        public Guid DiscountId { get; set; }
        public string DiscountType { get; set; }
        public string Percentage { get; set; }
    }
}
