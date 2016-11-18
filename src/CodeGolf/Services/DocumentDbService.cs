using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CodeGolf.Services
{
    public class DocumentDbService
    {
        private Dictionary<Guid, object> _cache = new Dictionary<Guid, object>();

        public DocumentDbService(DocumentDbConfig config)
        {
            _database = config.Database;
            _documentCollection = config.DocumentCollection;

            Client = new DocumentClient(new Uri(config.EndpointUri), config.PrimaryKey, new ConnectionPolicy { EnableEndpointDiscovery = false});

            Repository = new DocumentDbRepository(this, _database);
        }

        private readonly string _database;
        private readonly string _documentCollection;

        public DocumentClient Client { get; set; }

        public IRepository Repository { get; }

        public Uri DatabaseUri => UriFactory.CreateDocumentCollectionUri(_database, _documentCollection);

        internal async Task EnsureInitialized()
        {
            await Client.CreateDatabaseIfNotExists(_database);
            await Client.CreateDocumentCollectionIfNotExists(_database, _documentCollection);
            await Repository.Initialize();
        }

        internal async Task CreateDocument(CodeGolfDocument document)
        {
            document.Id = Guid.NewGuid();
            document.DateAdded = DateTime.UtcNow;
            document.DateModified = DateTime.UtcNow;

            await Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_database, _documentCollection), document);
        }

        internal T GetDocument<T>(Guid id, bool cache = false) where T : CodeGolfDocument
        {
            if (cache && _cache.ContainsKey(id))
            {
                return _cache[id] as T;
            }

            var document = 
                Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_database, _documentCollection))
                    .Where(m => m.Id == id).ToList().FirstOrDefault();

            if (cache)
            {
                _cache.Add(id, document);
            }

            return document;
        }

        internal async Task UpdateDocument(object document)
        {
            await Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_database, _documentCollection), document);
        }

        internal async Task DeleteDocument(Guid id)
        {
            await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_database, _documentCollection, id.ToString()));
        }

        internal IEnumerable<T> GetDocumentType<T>(DocumentType type) where T : CodeGolfDocument
        {
            return Client.CreateDocumentQuery<T>(DatabaseUri).Where(m => m.Type == type);
        }
    }

    public static class DocumentClientExtensions
    {
        /// <summary>
        /// Create a database with the specified name if it doesn't exist. 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="databaseName">The name/ID of the database.</param>
        /// <returns>The Task for asynchronous execution.</returns>
        public static async Task CreateDatabaseIfNotExists(this DocumentClient client, string databaseName)
        {
            // Check to verify a database with the id=FamilyDB_vg does not exist
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
            }
            catch (DocumentClientException de)
            {
                // If the database does not exist, create a new database
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = databaseName });
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Create a collection with the specified name if it doesn't exist.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="databaseName">The name/ID of the database.</param>
        /// <param name="collectionName">The name/ID of the collection.</param>
        /// <returns>The Task for asynchronous execution.</returns>
        public static async Task CreateDocumentCollectionIfNotExists(this DocumentClient client, string databaseName, string collectionName)
        {
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
            }
            catch (DocumentClientException de)
            {
                // If the document collection does not exist, create a new collection
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = collectionName;

                    // Optionally, you can configure the indexing policy of a collection. Here we configure collections for maximum query flexibility 
                    // including string range queries. 
                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

                    // DocumentDB collections can be reserved with throughput specified in request units/second. 1 RU is a normalized request equivalent to the read
                    // of a 1KB document.  Here we create a collection with 400 RU/s. 
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseName),
                        new DocumentCollection { Id = collectionName },
                        new RequestOptions { OfferThroughput = 400 });


                }
                else
                {
                    throw;
                }
            }
        }
    }
}
