using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CodeGolf.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CodeGolf.Services
{
    public class DocumentDbService
    {
        public DocumentDbService(DocumentDbConfig config)
        {
            _database = config.Database;
            _documentCollection = config.DocumentCollection;

            Client = new DocumentClient(new Uri(config.EndpointUri), config.PrimaryKey);
        }

        private readonly string _database;
        private readonly string _documentCollection;

        public DocumentClient Client { get; set; }

        public Uri DatabaseUri => UriFactory.CreateDocumentCollectionUri(_database, _documentCollection);

        /// <summary>
        /// Create a database with the specified name if it doesn't exist. 
        /// </summary>
        /// <param name="databaseName">The name/ID of the database.</param>
        /// <returns>The Task for asynchronous execution.</returns>
        internal async Task CreateDatabaseIfNotExists(string databaseName)
        {
            // Check to verify a database with the id=FamilyDB_vg does not exist
            try
            {
                await Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
            }
            catch (DocumentClientException de)
            {
                // If the database does not exist, create a new database
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await Client.CreateDatabaseAsync(new Database { Id = databaseName });
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
        /// <param name="databaseName">The name/ID of the database.</param>
        /// <param name="collectionName">The name/ID of the collection.</param>
        /// <returns>The Task for asynchronous execution.</returns>
        internal async Task CreateDocumentCollectionIfNotExists(string databaseName, string collectionName)
        {
            try
            {
                await Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
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
                    await Client.CreateDocumentCollectionAsync(
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

        internal async Task EnsureInitialized()
        {
            await CreateDatabaseIfNotExists(_database);
            await CreateDocumentCollectionIfNotExists(_database, _documentCollection);
        }

        internal async Task CreateDocument(CodeGolfDocument document)
        {
            document.Id = Guid.NewGuid();
            document.DateAdded = DateTime.UtcNow;
            document.DateModified = DateTime.UtcNow;

            await Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_database, _documentCollection), document);
        }

        internal T GetDocument<T>(Guid id) where T : CodeGolfDocument
        {
            return
                Client.CreateDocumentQuery<T>(UriFactory.CreateDocumentCollectionUri(_database, _documentCollection))
                    .Where(m => m.Id == id).ToList().FirstOrDefault();
        }

        internal async Task UpdateDocument(object document)
        {
            await Client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_database, _documentCollection), document);
        }

        internal async Task DeleteDocument(Guid id)
        {
            await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_database, _documentCollection, id.ToString()));
        }
    }

    public interface IDocumentDbService
    {
    }
}
