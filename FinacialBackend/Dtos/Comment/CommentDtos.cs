using FinacialBackend.Model;

namespace FinacialBackend.Dtos.Comment
{
    public class CommentDtos
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public int? StockId { get; set; }
        public string AppUserId { get; set; }
        //Navigation Property

    }
}
