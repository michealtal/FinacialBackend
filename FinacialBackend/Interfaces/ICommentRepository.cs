using FinacialBackend.Dtos.Comment;
using FinacialBackend.Helpers;
using FinacialBackend.Model;

namespace FinacialBackend.Interfaces
{
    public interface ICommentRepository
    {
       Task<List<Comment>> GetAllAsync(CommentQueryObject queryObject);
        Task<Comment?> GetByIdAsync(int id);    
        Task<Comment> CreateAsync(Comment commentModel);
        Task<Comment?> DeleteAsync(int Id);
        Task<Comment?> UpdateAsync(int id, Comment commentModel);
    }
}
