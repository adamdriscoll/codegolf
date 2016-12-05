using System.Collections.Generic;
using System.Linq;
using CodeGolf.Sql.Repository;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;

        public HomeController(IRepository repository)
        {
            _repository = repository;
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
            var popularProblems = new List<RecentProblem>();

            var problems = _repository.Problem.GetPopularProblems().ToList();

            foreach (var problem in problems)
            {
                var solutions = problem.Solutions;

                solutions = solutions.OrderBy(m => m.Passing != null && m.Passing.Value).ThenBy(m => m.Content.Length).ToList();

                var topSolution = solutions.FirstOrDefault();

                var topSolutionLength = 0;
                if (topSolution != null)
                    topSolutionLength = topSolution.Content.Length;

                popularProblems.Add(new RecentProblem
                {
                    Name = problem.Name,
                    Id = problem.ProblemId,
                    Language = problem.Language,
                    ShortestSolution = topSolutionLength,
                    SolutionCount = solutions.Count,
                    Author = problem.Author.Identity,
                    AuthorId = problem.Author.UserId.ToString()
                });
            }

            return popularProblems;
        }

        private IEnumerable<RecentProblem> GetRecentProblems()
        {
            var popularProblems = new List<RecentProblem>();

            var problems = _repository.Problem.GetRecentProblems().ToList();

            foreach (var problem in problems)
            {
                var solutions = problem.Solutions;

                solutions = solutions.OrderBy(m => m.Passing != null && m.Passing.Value).ThenBy(m => m.Content.Length).ToList();

                var topSolution = solutions.FirstOrDefault();

                var topSolutionLength = 0;
                if (topSolution != null)
                    topSolutionLength = topSolution.Content.Length;

                popularProblems.Add(new RecentProblem
                {
                    Name = problem.Name,
                    Id = problem.ProblemId,
                    Language = problem.Language,
                    ShortestSolution = topSolutionLength,
                    SolutionCount = solutions.Count,
                    Author = problem.Author.Identity,
                    AuthorId = problem.Author.UserId.ToString()
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
