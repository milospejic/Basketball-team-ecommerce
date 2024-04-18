using System.ComponentModel.DataAnnotations;

namespace backend.Models.Dtos
{
    public class DiscountUpdateDto : IValidatableObject
    {
        public string DiscountType { get; set; }
        public string Percentage { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (DiscountType != "13-18" && DiscountType != "18-30" && DiscountType != "30-45" && DiscountType != "45-60" && DiscountType != "60+" && DiscountType != "All")
            {
                yield return new ValidationResult(
                "DiscountType must be one of these values: ('13-18','18-30','30-45','45-60','60+','All')",
                new[] { "DiscountUpdateDto" });
            }

            if (Percentage != "10%" && Percentage != "20%" && Percentage != "30%" && Percentage != "40%" && Percentage != "50%")
            {
                yield return new ValidationResult(
                "Category of product must be one of these values: ('10%','20%','30%','40%','50%')",
                new[] { "DiscountUpdateDto" });
            }
        }
    }
}
