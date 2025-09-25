﻿using FinacialBackend.Data;
using FinacialBackend.Dtos.Comment;
using FinacialBackend.Helpers;
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

        public async Task<List<Comment>> GetAllAsync(CommentQueryObject queryObject)  
        {
            var comments = _context.Comments.Include(a => a.AppUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Symbol))
            {
                comments = comments.Where(s => s.Stock.Symbol == queryObject.Symbol); 
            };
            if (queryObject.IsDesecnding == true)
            {
                comments = comments.OrderByDescending(c => c.CreatedOn);
            }

            return await comments.ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.Include(a => a.AppUser).FirstOrDefaultAsync(c => c.Id == id);
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
