using System;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Interfaces.Repository;
using CodeGolf.Models;
using CodeGolf.Services;
using Microsoft.Azure.Documents.Client;
using NUnit.Framework;

namespace CodeGolf.Test.Services.Repository
{
    [TestFixture]
    public class ProblemRepositoryTest
    {
        private DocumentDbService _service;
        private IProblemRepository _repository;

        [SetUp]
        public async Task SetUp()
        {
            var documentDbEmulator = new DocumentDbEmulator();
            documentDbEmulator.Start();

            _service = new DocumentDbService(Constants.DocumentDbConfig);
            await _service.Client.CreateDatabaseIfNotExists(Constants.Database);

            _repository = _service.Repository.Problem;

            await _repository.Initialize();
        }

        [TearDown]
        public async Task TearDown()
        {
            var dbUri = UriFactory.CreateDatabaseUri(Constants.Database);
            await _service.Client.DeleteDatabaseAsync(dbUri);
        }

        [Test]
        public async Task ShouldInitializeProblemCollection()
        {
            await _service.Client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(Constants.Database, "Problems"));
        }

        [Test]
        public async Task ShouldGetPopularProblems()
        {
            await _repository.Create(new Problem
            {
                Id = Guid.NewGuid(),
                SolutionCount = 6,
                Name = "Problem1"
            });

            await _repository.Create(new Problem
            {
                Id = Guid.NewGuid(),
                SolutionCount = 3,
                Name = "Problem2"
            });

            var popProbs = await _repository.GetPopularProblems();

            Assert.AreEqual("Problem1", popProbs.First().Name);
            Assert.AreEqual("Problem2", popProbs.Last().Name);
        }

        [Test]
        public async Task ShouldGetRecentProblems()
        {
            await _repository.Create(new Problem
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.Now,
                SolutionCount = 6,
                Name = "Problem1"
            });

            await _repository.Create(new Problem
            {
                Id = Guid.NewGuid(),
                DateAdded = DateTime.Now.AddDays(-3),
                SolutionCount = 3,
                Name = "Problem2"
            });

            var popProbs = await _repository.GetPopularProblems();

            Assert.AreEqual("Problem1", popProbs.First().Name);
            Assert.AreEqual("Problem2", popProbs.Last().Name);
        }


    }
}
