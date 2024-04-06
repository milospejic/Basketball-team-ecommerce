namespace backend.Models.Dtos
{
    public class ReviewUpdateDto
    {
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}
