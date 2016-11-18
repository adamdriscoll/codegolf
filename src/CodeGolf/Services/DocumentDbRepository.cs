using System.Threading.Tasks;
using CodeGolf.Interfaces;

namespace CodeGolf.Services
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
        }

        public ICommentRepository Comments => new CommentRepository(_service.Client, _database);
    }
}
