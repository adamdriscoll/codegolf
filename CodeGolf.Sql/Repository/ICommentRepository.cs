using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Sql.Models;

namespace CodeGolf.Sql.Repository
{
    public interface ICommentRepository
    {
        IQueryable<SolutionComment> Get();

        Task<SolutionComment> GetSolutionComment(int id);

        IQueryable<SolutionComment> GetSolutionComments(int solutionId);

        Task<SolutionComment> AddSolutionComment(SolutionComment comment);

        Task DeleteSolutionComment(int commentId);
    }

    public class CommentRepository : ICommentRepository
    {
        private readonly CodeGolfDbContext _dbContext;

        public CommentRepository(CodeGolfDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<SolutionComment> Get()
        {
            return _dbContext.SolutionComments;
        }

        public async Task<SolutionComment> GetSolutionComment(int id)
        {
            return await _dbContext.SolutionComments.FirstOrDefaultAsync(m => m.SolutionCommentId == id);
        }

        public IQueryable<SolutionComment> GetSolutionComments(int solutionId)
        {
            return _dbContext.SolutionComments.Where(m => m.SolutionId == solutionId);
        }

        public async Task<SolutionComment> AddSolutionComment(SolutionComment comment)
        {
            comment.DateAdded = DateTime.UtcNow;
            comment = _dbContext.SolutionComments.Add(comment);
            await _dbContext.SaveChangesAsync();

            return comment;
        }

        public async Task DeleteSolutionComment(int commentId)
        {
            var comment = await GetSolutionComment(commentId);
            _dbContext.SolutionComments.Remove(comment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
