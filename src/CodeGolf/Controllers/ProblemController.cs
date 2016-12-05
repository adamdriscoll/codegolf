using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;
using CodeGolf.Sql.Repository;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class ProblemController : AuthorizedController
    {
        private readonly LanguageFactory _languageFactory;
        public ProblemController(IRepository repository, LanguageFactory factory) : base(repository)
        {
            _languageFactory = factory;
        }

        public async Task<IActionResult> Single(string problemName)
        {
            var problem = await Repository.Problem.Get(problemName);

            return ShowProblem(problem);
        }

        public async Task<IActionResult> Index(int id)
        {
            var problem = await Repository.Problem.Get(id);

            return ShowProblem(problem);
        }

        private IActionResult ShowProblem(Sql.Models.Problem problem)
        {
            if (problem == null) throw new Exception("Problem does not exist!");
            var author = problem.Author;

            var language = _languageFactory.Get(problem.Language);

            return View("Index", new ProblemDetails(problem, author, language, HttpContext.User.Identity.IsAuthenticated, HttpContext.User.Identity.Name));
        }

        public async Task<IEnumerable<SolutionDetail>> Solution(int id)
        {
            var currentUser = await GetRequestUser();

            var solutions = Repository.Solutions.GetSolutionByProblemId(id);
            
            var solutionDetails = new List<SolutionDetail>();
            foreach (var solution in solutions)
            {
                var currentUserName = currentUser?.Identity;
                var user = solution.Author;
                var userVm = new UserViewModel(user, currentUserName);
                var svm = new SolutionDetail(solution, userVm, Url);

                solutionDetails.Add(svm);
            }

            return solutionDetails.OrderByDescending(m => m.Votes).ThenBy(m => m.Passing != null && m.Passing.Value).ThenBy(m => m.Length).ToList();
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var problem = await Repository.Problem.Get(id);
            var user = await GetRequestUser();

            if (user.UserId != problem.Author.UserId)
                throw new Exception("User is not author!");

            var editProblem = new EditProblem(id, HttpContext.User.Identity.IsAuthenticated, HttpContext.User.Identity.Name);

            return View(editProblem);
        }

        [Authorize]
        [HttpPost]
        [Route("/problem/{id}")]
        public async Task<string> EditAsync(int id, Sql.Models.Problem problem)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));

            if (string.IsNullOrEmpty(problem.Description) || string.IsNullOrWhiteSpace(problem.Name) || !problem.TestCases.Any())
                throw new Exception("All details are required to create a problem");

            var user = await GetRequestUser();

            var existingProblem = await Repository.Problem.Get(id);

            if (existingProblem == null)
                throw new ArgumentNullException(nameof(problem));

            if (user.UserId != existingProblem.Author.UserId)
                throw new Exception("User is not author!");

            existingProblem.TestCases = problem.TestCases;
            existingProblem.Description = problem.Description;
            existingProblem.Language = problem.Language;
            existingProblem.Name = problem.Name;

            await Repository.SaveChangesAsync();

            return Url.Action("Index", new {problem.ProblemId});
        }


        [Authorize]
        [HttpPost]
        [Route("/problem/{id}/close")]
        public async Task Close(int id)
        {
            var problem = await Repository.Problem.Get(id);
            var user = await GetRequestUser();

            if (problem.Author.UserId != user.UserId)
            {
                throw new Exception("Current user is not author of problem!");
            }

            await Repository.Problem.Close(problem);
        }

        [Authorize]
        [HttpPost]
        [Route("/problem/")]
        public async Task<string> PostAsync(Sql.Models.Problem problem)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));

            if (problem.Language.Length > 15)  
                throw new Exception("Language name cannot be over 15 characters.");

            if (string.IsNullOrEmpty(problem.Description) || string.IsNullOrWhiteSpace(problem.Name)  || !problem.TestCases.Any())
                throw new Exception("All details are required to create a problem");

            var currentUser = await GetRequestUser();

            problem.Author = currentUser;

            await Repository.Problem.Create(problem);

            return Url.Action("Single", "Problem", new {problemName = problem.Name});
        }

        [HttpGet]
        public async Task<ICodeGolfLanguage> Language(int id)
        {
            var problem = await Repository.Problem.Get(id);

            return _languageFactory.Get(problem.Language);
        }

        [Authorize]
        public async Task<IActionResult> New()
        {
            var user = await GetRequestUser();
            var viewModel = new NewProblem(true, user.Identity);
           
            return View(viewModel);
        }

        [HttpGet]
        public IEnumerable<Sql.Models.Problem> Popular()
        {
            return Repository.Problem.GetPopularProblems().ToList();
        }

        public IActionResult Search( string criteria)
        {
            var recentProblems = SearchProblems(criteria);
            var vm = new SearchViewModel(criteria, recentProblems, HttpContext.User.Identity.IsAuthenticated, HttpContext.User.Identity.Name);

            return View(vm);
        }

        private IEnumerable<RecentProblem> SearchProblems(string critieria)
        {
            var recentProblems = new List<RecentProblem>();

            var problems = Repository.Problem.Find(critieria);

            foreach (var problem in problems)
            {
                var topSolution = problem.Solutions.OrderBy(m => m.Content.Length).FirstOrDefault();

                var topSolutionLength = 0;
                if (topSolution != null)
                    topSolutionLength = topSolution.Content.Length;

                recentProblems.Add(new RecentProblem
                {
                    Name = problem.Name,
                    Id = problem.ProblemId,
                    Language = problem.Language,
                    ShortestSolution = topSolutionLength,
                    SolutionCount = problem.Solutions.Count,
                    Author = problem.Author.Identity
                });
            }

            return recentProblems;
        }

        [HttpGet]
        public async Task<Sql.Models.Problem> Get(int id)
        {
            return await Repository.Problem.Get(id);
        }
    }
}
