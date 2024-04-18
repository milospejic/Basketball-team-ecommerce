using System.ComponentModel.DataAnnotations;

namespace backend.Models.Dtos
{
    public class ProductCreateDto : IValidatableObject
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string? Brand { get; set; }
        public string Category { get; set; }
        public string? Size { get; set; }
        public double Price { get; set; }
        public double TotalRating { get; set; }
        public int Quantity { get; set; }
        public int NumOfReviews { get; set; }
        public Guid AdminId { get; set; }
        public Guid? DiscountId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Price <= 0)
            {
                yield return new ValidationResult(
                    "Price of product must be bigger than 0",
                    new[] { "ProductCreateDto" });
            }

            if (Quantity <= 0)
            {
                yield return new ValidationResult(
                    "Quantity of product must be bigger than 0",
                    new[] { "ProductCreateDto" });
            }

            if (Category == "SeasonTicket")
            {
                if (Size != null)
                {
                    yield return new ValidationResult(
                    "SeasonTicket can not have Size value",
                    new[] { "ProductCreateDto" });
                }
            }
            if (Size != null)
            {
                if(Size != "10" && Size != "12" && Size != "XS" && Size != "S" && Size != "M" && Size != "L" && Size != "XL" && Size != "XXL" && Size != "XXXL")
                {
                    yield return new ValidationResult(
                   "Size of product must be one of these values: ('10','12','XS','S','M','L','XL','XXL','XXXL')",
                   new[] { "ProductCreateDto" });
                }
            }
  
            if (Category != "SeasonTicket" && Category != "Jersey" && Category != "Shorts" && Category != "T-shirt" && Category != "Cap" && Category != "Hoody")
            {
                yield return new ValidationResult(
                "Category of product must be one of these values: ('SeasonTicket','Jersey','Shorts','T-shirt','Cap','Hoody')",
                new[] { "ProductCreateDto" });
            }
            
        }
    }
}
