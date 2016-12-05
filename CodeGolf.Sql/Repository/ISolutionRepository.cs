using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeGolf.Sql.Models;

namespace CodeGolf.Sql.Repository
{
    public interface ISolutionRepository
    {
        IEnumerable<Solution> GetSolutionByProblemId(int problemId);
        Task<Solution> Get(int id);
        Task Delete(Solution solution);
        Task Create(Solution solution);
    }

    public class SolutionRepository : ISolutionRepository
    {
        private readonly CodeGolfDbContext _dbContext;

        public SolutionRepository(CodeGolfDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Solution> GetSolutionByProblemId(int problemId)
        {
            return _dbContext.Solutions.Where(m => m.ProblemId == problemId);
        }

        public async Task<Solution> Get(int id)
        {
            return await _dbContext.Solutions.FirstOrDefaultAsync(m => m.SolutionId == id);
        }

        public async Task Delete(Solution solution)
        {
            _dbContext.SolutionComments.RemoveRange(solution.SolutionComments);
            _dbContext.Votes.RemoveRange(solution.Votes);
            _dbContext.Solutions.Remove(solution);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Create(Solution solution)
        {
            solution.DateAdded = DateTime.UtcNow;
            _dbContext.Solutions.Add(solution);
            await _dbContext.SaveChangesAsync();
        }
    }
}
