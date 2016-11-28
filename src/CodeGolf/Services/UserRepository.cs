using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Models;
using Microsoft.Azure.Documents.Client;

namespace CodeGolf.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly DocumentClient _client;
        private readonly string _databaseName;
        private const string Collection = "Users";
        private readonly Uri _collectionUri;

        public UserRepository(DocumentClient client, string databaseName)
        {
            _client = client;
            _databaseName = databaseName;
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, Collection);
        }

        public async Task Initialize()
        {
            await _client.CreateDocumentCollectionIfNotExists(_databaseName, Collection);
        }

        public async Task<User> Get(Guid id)
        {
            return _client.CreateDocumentQuery<User>(_collectionUri).Where(m => m.Id == id).ToList().FirstOrDefault();
        }

        public async Task<IEnumerable<User>> Find(string name)
        {
            //TODO: Probably should do a regex search
            return _client.CreateDocumentQuery<User>(_collectionUri).Where(m => m.Identity == name);
        }

        public async Task<User> Get(string identity, string provider)
        {
            return _client.CreateDocumentQuery<User>(_collectionUri).Where(m => m.Identity == identity && m.Authentication == provider).ToList().FirstOrDefault();
        }

        public async Task Create(User user)
        {
            await _client.CreateDocumentAsync(_collectionUri, user);
        }
    }
}
