using System;
using System.Collections.Generic;
using System.Linq;
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

        public IActionResult Index()
        {
            var recentProblems = GetRecentProblems();
            var popularProblems = GetPopularProblems();
            var vm = new HomeViewModel(recentProblems, popularProblems, HttpContext.User.Identity.IsAuthenticated,
                HttpContext.User.Identity.Name);

            return View(vm);
        }

        private IEnumerable<RecentProblem> GetPopularProblems()
        {
            var problems = _documentDbService.Client.CreateDocumentQuery<Problem>(_documentDbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.Problem)
                .OrderByDescending(m => m.SolutionCount)
                .Take(10).ToList();

            foreach (var problem in problems)
            {
                var language = _documentDbService.GetDocument<Language>(problem.Language);
                var author = _documentDbService.GetDocument<User>(problem.Author);

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

                yield return new RecentProblem
                {
                    Name = problem.Name,
                    Id = problem.Id,
                    Language = language.DisplayName,
                    ShortestSolution = topSolutionLength,
                    SolutionCount = solutions.Count,
                    Author = author.Identity
                };
            }
        }

        private IEnumerable<RecentProblem> GetRecentProblems()
        {
            var problems = _documentDbService.Client.CreateDocumentQuery<Problem>(_documentDbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.Problem)
                .OrderByDescending(m => m.DateAdded)
                .Take(10).ToList();

            foreach (var problem in problems)
            {
                var language = _documentDbService.GetDocument<Language>(problem.Language);
                var author = _documentDbService.GetDocument<User>(problem.Author);
                
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

                yield return new RecentProblem
                {
                    Name = problem.Name,
                    Id = problem.Id,
                    Language = language.DisplayName,
                    ShortestSolution = topSolutionLength,
                    SolutionCount = solutions.Count,
                    Author = author.Identity
                };
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
