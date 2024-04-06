namespace backend.Models.Dtos
{
    public class ReviewDto
    {
        public Guid ReviewId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}
