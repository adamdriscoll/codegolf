using System.Data.Entity;
using System.Threading.Tasks;
using CodeGolf.Sql.Models;

namespace CodeGolf.Sql.Repository
{
    public interface IVoteRepository
    {
        Task<Vote> GetVoteByItemIdAndUser(int itemId, int userId);
        Task Create(Vote vote);
    }

    public class VoteRepository : IVoteRepository
    {
        private readonly CodeGolfDbContext _dbContext;

        public VoteRepository(CodeGolfDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Vote> GetVoteByItemIdAndUser(int itemId, int userId)
        {
            return await _dbContext.Votes.FirstOrDefaultAsync(m => m.ItemId == itemId && m.VoterId == userId);
        }

        public async Task Create(Vote vote)
        {
            _dbContext.Votes.Add(vote);
            await _dbContext.SaveChangesAsync(); 
        }
    }
}
