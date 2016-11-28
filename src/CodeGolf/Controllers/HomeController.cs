using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;
using CodeGolf.Services;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class HomeController : Controller
    {
        private readonly DocumentDbService _documentDbService;

        public HomeController(DocumentDbService dbService)
        {
            _documentDbService = dbService;
        }

        public async Task<IActionResult> Index()
        {
            var recentProblems = await GetRecentProblems();
            var popularProblems = await GetPopularProblems();
            var vm = new HomeViewModel(recentProblems, popularProblems, HttpContext.User.Identity.IsAuthenticated,
                HttpContext.User.Identity.Name);

            return View(vm);
        }

        private async Task<IEnumerable<RecentProblem>> GetPopularProblems()
        {
            var popularProblems = new List<RecentProblem>();

            var problems = _documentDbService.Client.CreateDocumentQuery<Problem>(_documentDbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.Problem)
                .OrderByDescending(m => m.SolutionCount)
                .Take(10).ToList();

            foreach (var problem in problems)
            {
                var language = _documentDbService.GetDocument<Language>(problem.Language, true);
                var author = await _documentDbService.Repository.Users.Get(problem.Author);

                if (language == null)
                    throw new Exception("Language cannot be null.");

                var solutions = _documentDbService.Client.CreateDocumentQuery<Solution>(_documentDbService.DatabaseUri)
                    .Where(m => problem.Solutions.Contains(m.Id))
                    .ToList();

                solutions = solutions.OrderBy(m => m.Passing != null && m.Passing.Value).ThenBy(m => m.Length).ToList();

                var topSolution = solutions.FirstOrDefault();

                var topSolutionLength = 0;
                if (topSolution != null)
                    topSolutionLength = topSolution.Length;

                popularProblems.Add(new RecentProblem
                {
                    Name = problem.Name,
                    Id = problem.Id,
                    Language = language.DisplayName,
                    ShortestSolution = topSolutionLength,
                    SolutionCount = solutions.Count,
                    Author = author.Identity,
                    AuthorId = author.Id.ToString()
                });
            }

            return popularProblems;
        }

        private async Task<IEnumerable<RecentProblem>> GetRecentProblems()
        {
            var popularProblems = new List<RecentProblem>();

            var problems = _documentDbService.Client.CreateDocumentQuery<Problem>(_documentDbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.Problem)
                .OrderByDescending(m => m.DateAdded)
                .Take(10).ToList();

            foreach (var problem in problems)
            {
                var language = _documentDbService.GetDocument<Language>(problem.Language);
                var author = await _documentDbService.Repository.Users.Get(problem.Author);

                if (language == null)
                    throw new Exception("Language cannot be null.");

                var solutions = _documentDbService.Client.CreateDocumentQuery<Solution>(_documentDbService.DatabaseUri)
                    .Where(m => problem.Solutions.Contains(m.Id))
                    .ToList();

                solutions = solutions.OrderBy(m => m.Passing != null && m.Passing.Value).ThenBy(m => m.Length).ToList();

                var topSolution = solutions.FirstOrDefault();

                var topSolutionLength = 0;
                if (topSolution != null)
                    topSolutionLength = topSolution.Length;

                popularProblems.Add(new RecentProblem
                {
                    Name = problem.Name,
                    Id = problem.Id,
                    Language = language.DisplayName,
                    ShortestSolution = topSolutionLength,
                    SolutionCount = solutions.Count,
                    Author = author.Identity,
                    AuthorId = author.Id.ToString()
                });
            }

            return popularProblems;
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
