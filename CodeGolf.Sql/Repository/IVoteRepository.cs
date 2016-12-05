using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Sql.Models;

namespace CodeGolf.Sql.Repository
{
    public interface IVoteRepository
    {
        Task<Vote> GetVoteByItemIdAndUser(int itemId, int userId);
        Task<int> GetCountForItemId(int itemId);
        Task DeleteForItemId(int itemId);
        Task Create(Vote vote);
    }

    public class VoteRepository : IVoteRepository
    {
        private readonly CodeGolfDbContext _dbContext;

        public VoteRepository(CodeGolfDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteForItemId(int itemId)
        {
            _dbContext.Votes.RemoveRange(_dbContext.Votes.Where(m => m.ItemId == itemId).ToList());
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Vote> GetVoteByItemIdAndUser(int itemId, int userId)
        {
            return await _dbContext.Votes.FirstOrDefaultAsync(m => m.ItemId == itemId && m.VoterId == userId);
        }

        public async Task<int> GetCountForItemId(int itemId)
        {
            int? v = await _dbContext.Votes.Where(m => m.ItemId == itemId).SumAsync(m => (int?)m.Value);
            return v.GetValueOrDefault(0);
        }

        public async Task Create(Vote vote)
        {
            _dbContext.Votes.Add(vote);
            await _dbContext.SaveChangesAsync(); 
        }
    }
}
