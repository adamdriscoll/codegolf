using System;
using System.Threading.Tasks;
using CodeGolf.Sql.Models;
using CodeGolf.Sql.Repository;
using NUnit.Framework;

namespace CodeGolf.Sql.Test.Repository
{
    [TestFixture]
    public class ProblemRepositoryTest
    {
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
