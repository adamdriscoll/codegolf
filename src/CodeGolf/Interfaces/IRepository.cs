using System.Threading.Tasks;

namespace CodeGolf.Interfaces
{
    public interface IRepository
    {
        Task Initialize();
        ICommentRepository Comments { get; }
    }
}
