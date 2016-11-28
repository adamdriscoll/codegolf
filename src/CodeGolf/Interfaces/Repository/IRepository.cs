using System.Threading.Tasks;

namespace CodeGolf.Interfaces.Repository
{
    public interface IRepository
    {
        Task Initialize();
        ICommentRepository Comments { get; }

        IUserRepository Users { get; }

        IProblemRepository Problem { get; }
    }
}
