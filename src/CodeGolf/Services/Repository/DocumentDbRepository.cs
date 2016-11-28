using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Interfaces.Repository;

namespace CodeGolf.Services.Repository
{
    public class DocumentDbRepository : IRepository
    {
        private readonly DocumentDbService _service;
        private readonly string _database;

        public DocumentDbRepository(DocumentDbService service, string database)
        {
            _service = service;
            _database = database;
        }

        public async Task Initialize()
        {
            await Comments.Initialize();
            await Users.Initialize();
            await Problem.Initialize();
        }

        public ICommentRepository Comments => new CommentRepository(_service.Client, _database);
        public IUserRepository Users => new UserRepository(_service.Client, _database);
        public IProblemRepository Problem => new ProblemRepository(_service.Client, _database);
    }
}
