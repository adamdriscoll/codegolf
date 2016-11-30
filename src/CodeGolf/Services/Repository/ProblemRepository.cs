using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Interfaces.Repository;
using CodeGolf.Models;
using Microsoft.Azure.Documents.Client;

namespace CodeGolf.Services.Repository
{
    public class ProblemRepository : IProblemRepository
    {
        private readonly DocumentClient _client;
        private readonly string _databaseName;
        private const string Collection = "Problems";
        private readonly Uri _collectionUri;

        public ProblemRepository(DocumentClient client, string databaseName)
        {
            _client = client;
            _databaseName = databaseName;
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, Collection);
        }

        public async Task Initialize()
        {
            await _client.CreateDocumentCollectionIfNotExists(_databaseName, Collection);
        }

        public async Task Create(Problem problem)
        {
            await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, Collection), problem);
        }

        public async Task<IEnumerable<Problem>> Get()
        {
            return _client.CreateDocumentQuery<Problem>(_collectionUri);
        }


        public async Task<Problem> Get(string name)
        {
            return _client.CreateDocumentQuery<Problem>(_collectionUri).Where(m => m.Name.ToLower() == name.ToLower()).ToList().FirstOrDefault();
        }

        public async Task<Problem> Get(Guid id)
        {
            return _client.CreateDocumentQuery<Problem>(_collectionUri).Where(m => m.Id == id).ToList().FirstOrDefault();
        }

        public async Task<IEnumerable<Problem>> Find(string text)
        {
            return _client.CreateDocumentQuery<Problem>(_collectionUri).ToList()
               .Where(m => (m.Name.ToLower().Contains(text.ToLower()) || m.Description.ToLower().Contains(text.ToLower())))
               .OrderByDescending(m => m.DateAdded)
               .Take(10);
        }

        public async Task Update(Problem problem)
        {
            await _client.UpsertDocumentAsync(_collectionUri, problem);
        }

        public async Task Close(Problem problem)
        {
            problem.Closed = true;
            await Update(problem);
        }

        public async Task<IEnumerable<Problem>> GetPopularProblems()
        {
            return _client.CreateDocumentQuery<Problem>(_collectionUri)
                            .OrderByDescending(m => m.SolutionCount)
                            .Take(10).ToList();
        }

        public async Task<IEnumerable<Problem>> GetRecentProblems()
        {
            return _client.CreateDocumentQuery<Problem>(_collectionUri)
                             .Where(m => m.Type == DocumentType.Problem)
                             .OrderByDescending(m => m.DateAdded)
                             .Take(10).ToList();
        }

        public async Task<IEnumerable<Problem>> GetByUser(User user)
        {
            return _client.CreateDocumentQuery<Problem>(UriFactory.CreateDocumentCollectionUri(_databaseName, Collection)).Where(m => m.Author == user.Id);
        }
    }
}
