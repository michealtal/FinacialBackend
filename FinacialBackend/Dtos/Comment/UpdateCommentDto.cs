using System.ComponentModel.DataAnnotations;

namespace FinacialBackend.Dtos.Comment
{
    public class UpdateCommentDto
    {
        [Required]
        [MinLength(5, ErrorMessage = "Title must be 5 chracters")]
        [MaxLength(280, ErrorMessage = "Title Cannot be over 280 chracters")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(5, ErrorMessage = "Content must be 5 chracters")]
        [MaxLength(280, ErrorMessage = "Content Cannot be over 280 chracters")]
        public string Content { get; set; } = string.Empty;
    }
}
