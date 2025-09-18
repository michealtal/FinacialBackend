using FinacialBackend.Data;
using FinacialBackend.Dtos.Comment;
using FinacialBackend.Interfaces;
using FinacialBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace FinacialBackend.Repository
{
   
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDBContext _context;
        public CommentRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Comment> CreateAsync(Comment commentModel)
        {
            await _context.Comments.AddAsync(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var CommentModel = await _context.Comments.FirstOrDefaultAsync(x =>x.Id == id);

            if (CommentModel == null)
            {
                return null;
            }
            _context.Comments.Remove(CommentModel);
            await _context.SaveChangesAsync();
            return CommentModel;
        }

        public async Task<List<Comment>> GetAllAsync()  
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<Comment> UpdateAsync(int id, Comment commentModel)
        {
            var existingComment = await _context.Comments.FindAsync(id);

            if (existingComment == null)
            {
                return null;
            }

            existingComment.Title = commentModel.Title;
            existingComment.Content = commentModel.Content;

            await _context.SaveChangesAsync();
            return existingComment;

        }
    }
}
