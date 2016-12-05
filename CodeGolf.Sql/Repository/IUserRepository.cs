using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Sql.Models;

namespace CodeGolf.Sql.Repository
{
    public interface IUserRepository
    {
        IEnumerable<User> Get();
        Task<User> Get(int id);
        IEnumerable<User> Find(string name);
        Task<User> Get(string identity, string provider);
        Task Create(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly CodeGolfDbContext _dbContext;

        public UserRepository(CodeGolfDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<User> Get()
        {
            return _dbContext.Users;
        }

        public async Task<User> Get(int id)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(m => m.UserId == id);
        }

        public IEnumerable<User> Find(string name)
        {
            return _dbContext.Users.Where(m => m.Identity.Contains(name));
        }

        public async Task<User> Get(string identity, string provider)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(m => m.Identity == identity && m.Authentication == provider);
        }

        public async Task Create(User user)
        {
            user.DateAdded = DateTime.UtcNow;
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
