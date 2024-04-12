using backend.Models.Dtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public int NumberOfItems { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

        [NotMapped]
        public List<ProductsInOrderDto> ProductsInOrder { get; set; }
    }

}
