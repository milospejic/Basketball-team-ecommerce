﻿namespace backend.Models.Dtos
{
    public class ProductOrderDto
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Amount { get; set; }
    }
}
