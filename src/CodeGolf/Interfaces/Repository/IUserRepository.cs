using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeGolf.Models;

namespace CodeGolf.Interfaces.Repository
{
    public interface IUserRepository
    {
        Task Initialize();
        Task<User> Get(Guid id);
        Task<IEnumerable<User>> Find(string name);
        Task<User> Get(string identity, string provider);
        Task Create(User user);
    }
}
