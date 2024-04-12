using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class Review
    {
        public Guid ReviewId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public User User { get; set; }
        public Product Product { get; set; }

        [NotMapped]
        public bool? IsDeleted { get; set; } = false;
    }
}
