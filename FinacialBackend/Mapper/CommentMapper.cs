using FinacialBackend.Dtos.Comment;
using FinacialBackend.Model;

namespace FinacialBackend.Mapper
{
    public static class CommentMapper
    {
        public static CommentDtos ToCommentDto(this Comment commentModel)
        {
            return new CommentDtos
            {
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                CreatedBy = commentModel.AppUser?.UserName ?? "unknown",
                StockId = commentModel.StockId,
                AppUserId = commentModel.AppUserId
            };
        }

        public static Comment ToCommentFromCreate(this CreateCommentDto commentDto, int stockId)
        {
            return new Comment
            {
                Title = commentDto.Title,
                Content = commentDto.Content,
                StockId = stockId
            }; 
        }

        public static Comment ToCommentFromUpdate(this UpdateCommentDto commentDto)
        {
            return new Comment
            {
                Title = commentDto.Title,
                Content = commentDto.Content,
            };
        }
    }
}
