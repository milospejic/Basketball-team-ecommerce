﻿namespace backend.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public string Size { get; set; }
        public float Price { get; set; }
        public float TotalRating { get; set; }
        public int Quantity { get; set; }
        public int NumOfReviews { get; set; }
        public Guid AdminId { get; set; }
        public Guid? DiscountId { get; set; }
        public Discount Discount { get; set; }
    }
}
