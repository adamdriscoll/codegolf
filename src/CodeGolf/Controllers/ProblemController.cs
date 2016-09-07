using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;
using CodeGolf.Services;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CodeGolf.Controllers
{
    public class ProblemController : Controller
    {
        private readonly DocumentDbService _documentDbService;
        public ProblemController(DocumentDbService dbService)
        {
            _documentDbService = dbService;
        }

        // GET: /<controller>/
        public IActionResult Index(Guid id)
        {
            var problem = _documentDbService.GetDocument<Problem>(id);
            var author = _documentDbService.GetDocument<User>(problem.Author);
            var language = _documentDbService.GetDocument<Language>(problem.Language);

            if (problem == null) throw new Exception("Problem does not exist!");

            var solutions = new List<SolutionDetail>();
            foreach (var solutionId in problem.Solutions)
            {
                var solution = _documentDbService.GetDocument<Solution>(solutionId);
                if (solution != null)
                {
                    var user = _documentDbService.GetDocument<User>(solution.Author);
                    var userIdentity = String.Empty;
                    if (user != null)
                        userIdentity = user.Identity;

                    var svm = new SolutionDetail(solution, userIdentity);

                    solutions.Add(svm);
                }
            }

            solutions = solutions.OrderBy(m => m.Passing != null && m.Passing.Value).ThenBy(m => m.Length).ToList();

            return View(new ProblemDetails(problem, solutions, author.Identity, language, HttpContext.User.Identity.IsAuthenticated,
                HttpContext.User.Identity.Name));
        }

        public async Task<IActionResult> PostAsync(Problem problem)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));

            if (string.IsNullOrEmpty(problem.Description) || string.IsNullOrWhiteSpace(problem.Name)  || string.IsNullOrWhiteSpace(problem.Output))
                throw new Exception("All details are required to create a problem");

            var user = _documentDbService.Client.CreateDocumentQuery<User>(_documentDbService.DatabaseUri).Where(m => m.Identity == this.HttpContext.User.Identity.Name && m.Authentication == this.HttpContext.User.Identity.AuthenticationType).ToList().FirstOrDefault();
            if (user == null)
            {
                user = new User
                {
                    Identity = this.HttpContext.User.Identity.Name,
                    Authentication = this.HttpContext.User.Identity.AuthenticationType
                };

                await _documentDbService.CreateDocument(user);
            }

            problem.Author = user.Id;

            await _documentDbService.CreateDocument(problem);

            return Redirect("/Problem/Index/" + problem.Id);
        }

        [HttpGet]
        public Language Language(Guid id)
        {
            var problem = _documentDbService.Client.CreateDocumentQuery<Problem>(_documentDbService.DatabaseUri)
                .FirstOrDefault(m => m.Id == id);

            return _documentDbService.Client.CreateDocumentQuery<Language>(_documentDbService.DatabaseUri)
               .FirstOrDefault(m => m.Id == problem.Language);
        }

        [Authorize]
        public IActionResult New()
        {
            var viewModel = new NewProblem(HttpContext.User.Identity.IsAuthenticated,
                HttpContext.User.Identity.Name)
            {
                Languages = _documentDbService.Client.CreateDocumentQuery<Language>(_documentDbService.DatabaseUri).Where(m => m.Type == DocumentType.Language).OrderBy(m => m.DisplayName)
            };
           
            return View(viewModel);
        }

        [HttpGet]
        public IEnumerable<Problem> Popular()
        {
            
            var list = _documentDbService.Client.CreateDocumentQuery<Problem>(_documentDbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.Problem && m.Solutions.Length > 5)
                .OrderByDescending(m => m.DateAdded)
                .ThenByDescending(m => m.Solutions.Length)
                .Take(10).ToList();

            return list;
        }

        public IActionResult Search( string criteria)
        {
            var recentProblems = SearchProblems(criteria);
            var vm = new SearchViewModel(criteria, recentProblems, HttpContext.User.Identity.IsAuthenticated,
                HttpContext.User.Identity.Name);

            return View(vm);
        }

        private IEnumerable<RecentProblem> SearchProblems(string critieria)
        {
            var problems = _documentDbService.Client.CreateDocumentQuery<Problem>(_documentDbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.Problem && (m.Name.Contains(critieria) || m.Description.Contains(critieria)))
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
                    .OrderBy(m => m.Length)
                    .ToList();

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

        [HttpGet]
        public Problem Get(Guid id)
        {
            return _documentDbService.GetDocument<Problem>(id);
        }
    }
}
