using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace CodeGolf.Sql.Repository
{
    public interface IRepository
    {
        ICommentRepository Comments { get; }

        IUserRepository Users { get; }

        IProblemRepository Problem { get; }

        ISolutionRepository Solutions { get; }

        IVoteRepository Votes { get; }

        Task SaveChangesAsync();
    }

    public class Repository : IRepository, IDisposable
    {
        private CodeGolfDbContext _dbContext;

        public Repository(string connectionString)
        {
            _dbContext = new CodeGolfDbContext(connectionString);
        }

        public ICommentRepository Comments => new CommentRepository(_dbContext);
        public IUserRepository Users => new UserRepository(_dbContext);
        public IProblemRepository Problem => new ProblemRepository(_dbContext);
        public ISolutionRepository Solutions => new SolutionRepository(_dbContext);
        public IVoteRepository Votes => new VoteRepository(_dbContext);
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
                _dbContext = null;
            }
        }
    }
}
