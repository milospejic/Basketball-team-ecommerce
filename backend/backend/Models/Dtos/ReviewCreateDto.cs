using System.ComponentModel.DataAnnotations;

namespace backend.Models.Dtos
{
    public class ReviewCreateDto : IValidatableObject
    {
        public string ReviewText { get; set; }
        public int Rating { get; set; }
       // public Guid UserId { get; set; }
        public Guid ProductId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Rating < 1 && Rating > 5)
            {
                yield return new ValidationResult(
                    "Rating must be between 1 and 5",
                    new[] { "RatingCreateDto" });
            }
        }
    }
}
