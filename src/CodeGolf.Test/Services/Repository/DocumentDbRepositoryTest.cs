using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;
using CodeGolf.Services;
using CodeGolf.Services.Repository;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;

namespace CodeGolf.Test.Services.Repository
{
    [TestFixture]
    public class DocumentDbRepositoryTest
    {
        private DocumentDbService _service;

        [SetUp]
        public async Task SetUp()
        {
            var documentDbEmulator = new DocumentDbEmulator();
            documentDbEmulator.Start();

            _service = new DocumentDbService(Constants.DocumentDbConfig);
            await _service.Client.CreateDatabaseIfNotExists(Constants.Database);
        }

        [TearDown]
        public async Task TearDown()
        {
            var dbUri = UriFactory.CreateDatabaseUri(Constants.Database);
            await _service.Client.DeleteDatabaseAsync(dbUri);
        }

        [Test]
        public async Task ShouldInitializeUserCollection()
        {
            await _service.Repository.Initialize();

            await _service.Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(Constants.Database, "Users"));
        }

        [Test]
        public async Task ShouldInitializeProblemCollection()
        {
            await _service.Repository.Initialize();

            await _service.Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(Constants.Database, "Problems"));
        }

        [Test]
        public async Task ShouldInitializeCommentCollection()
        {
            await _service.Repository.Initialize();

            await _service.Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(Constants.Database, "Comments"));
        }

    }
}
