namespace backend.Models.Dtos
{
    public class OrderUpdateDto
    {
        public int NumberOfItems { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
    }
}
