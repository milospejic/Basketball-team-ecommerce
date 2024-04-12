namespace backend.Models.Dtos
{
    public class OrderUpdateDto
    {
        public string OrderStatus { get; set; }
        public List<ProductsInOrderDto> ProductsInOrder { get; set; }
    }
}
