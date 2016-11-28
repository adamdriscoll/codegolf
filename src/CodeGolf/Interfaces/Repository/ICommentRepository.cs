using System;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;

namespace CodeGolf.Interfaces.Repository
{
    public interface ICommentRepository
    {
        Task Initialize();

        SolutionComment GetSolutionComment(Guid id);

        IQueryable<SolutionComment> GetSolutionComments(Guid solutionId);

        Task<string> AddSolutionComment(SolutionComment comment);

        Task DeleteSolutionComment(Guid commentId);
    }
}
