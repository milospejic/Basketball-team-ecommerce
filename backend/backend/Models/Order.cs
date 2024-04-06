namespace backend.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public int NumberOfItems { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public float TotalPrice { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
    }

}
