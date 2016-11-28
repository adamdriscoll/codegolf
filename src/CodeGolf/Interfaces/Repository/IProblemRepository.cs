using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeGolf.Models;

namespace CodeGolf.Interfaces.Repository
{
    public interface IProblemRepository
    {
        Task Initialize();
        Task Create(Problem problem);
        Task<Problem> Get(string name);
        Task<Problem> Get(Guid id);
        Task<IEnumerable<Problem>> Find(string text);
        Task Update(Problem problem);
        Task Close(Problem problem);
        Task<IEnumerable<Problem>> GetPopularProblems();
        Task<IEnumerable<Problem>> GetRecentProblems();
        Task<IEnumerable<Problem>> GetByUser(User user);
    }
}
