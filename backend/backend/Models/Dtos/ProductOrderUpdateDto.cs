using System.ComponentModel.DataAnnotations;

namespace backend.Models.Dtos
{
    public class ProductOrderUpdateDto : IValidatableObject
    {
        public int Amount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Amount <= 0)
            {
                yield return new ValidationResult(
                    "Amount must be bigger than 0",
                    new[] { "ProductOrderCreateDto" });
            }
        }
    }
}
