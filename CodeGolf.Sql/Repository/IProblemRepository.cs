using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Sql.Models;

namespace CodeGolf.Sql.Repository
{
    public interface IProblemRepository
    {
        Task Create(Problem problem);
        IQueryable<Problem> Get();
        Task<Problem> Get(string name);
        Task<Problem> Get(int id);
        IEnumerable<Problem> Find(string text);
        Task Close(Problem problem);
        IEnumerable<Problem> GetPopularProblems();
        IEnumerable<Problem> GetRecentProblems();
        IEnumerable<Problem> GetByUser(User user);
    }

    public class ProblemRepository : IProblemRepository
    {
        private readonly CodeGolfDbContext _dbContext;

        public ProblemRepository(CodeGolfDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Create(Problem problem)
        {
            problem.DateAdded = DateTime.UtcNow;
            _dbContext.Problems.Add(problem);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<Problem> Get()
        {
            return _dbContext.Problems;
        }

        public async Task<Problem> Get(string name)
        {
            return await _dbContext.Problems.FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task<Problem> Get(int id)
        {
            return await _dbContext.Problems.FirstOrDefaultAsync(m => m.ProblemId == id);
        }

        public IEnumerable<Problem> Find(string text)
        {
            return
                _dbContext.Problems.Where(
                    m => m.Name.Contains(text) || m.Description.Contains(text) || m.Language.Contains(text));
        }

        public Task Close(Problem problem)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Problem> GetPopularProblems()
        {
            return _dbContext.Problems.OrderBy(m => m.Solutions.Count).Take(10);
        }

        public IEnumerable<Problem> GetRecentProblems()
        {
            return _dbContext.Problems.OrderByDescending(m => m.DateAdded).Take(10);
        }

        public IEnumerable<Problem> GetByUser(User user)
        {
            return user.Problems;
        }
    }
}
