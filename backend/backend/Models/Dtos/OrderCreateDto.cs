﻿namespace backend.Models.Dtos
{
    public class OrderCreateDto
    {
       // public int NumberOfItems { get; set; }
       // public string OrderStatus { get; set; }
       // public DateTime OrderDate { get; set; }
        //public double TotalPrice { get; set; }
       // public Guid UserId { get; set; }
        public List<ProductsInOrderDto> ProductsInOrder { get; set; }
    }
}
