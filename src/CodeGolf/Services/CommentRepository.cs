using System;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Models;
using Microsoft.Azure.Documents.Client;

namespace CodeGolf.Services
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DocumentClient _client;
        private readonly string _databaseName;
        private const string Collection = "Comments";
        private readonly Uri _collectionUri;

        public CommentRepository(DocumentClient client, string databaseName)
        {
            _client = client;
            _databaseName = databaseName;
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, Collection);

        }

        public async Task Initialize()
        {
            await _client.CreateDocumentCollectionIfNotExists(_databaseName, Collection);
        }

        public IQueryable<SolutionComment> GetSolutionComments(Guid solutionId)
        {
            return _client.CreateDocumentQuery<SolutionComment>(_collectionUri).Where(m => m.Solution == solutionId);
        }

        public SolutionComment GetSolutionComment(Guid id)
        {
            return _client.CreateDocumentQuery<SolutionComment>(_collectionUri).Where(m => m.Id == id).ToList().FirstOrDefault();
        }

        public async Task<string> AddSolutionComment(SolutionComment comment)
        {
            comment.Id = Guid.NewGuid();
            var document = await _client.CreateDocumentAsync(_collectionUri, comment);
            return document.Resource.Id;
        }

        public async Task DeleteSolutionComment(Guid commentId)
        {
            var documentId = UriFactory.CreateDocumentUri(_databaseName, Collection, commentId.ToString());
            await _client.DeleteDocumentAsync(documentId);
        }
    }
}
