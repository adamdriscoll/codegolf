using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CodeGolf.Models;
using CodeGolf.Services;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class ProblemController : AuthorizedController
    {
        public ProblemController(DocumentDbService dbService) : base(dbService)
        {
        }

        public IActionResult Single(string problemName)
        {
            var problem = DocumentDbService.Client.CreateDocumentQuery<Problem>(DocumentDbService.DatabaseUri)
                .Where(m => m.Name.ToLower() == problemName.ToLower() && m.Type == DocumentType.Problem).ToList().FirstOrDefault();

            return ShowProblem(problem);
        }

        // GET: /<controller>/
        public IActionResult Index(Guid id)
        {
            var problem = DocumentDbService.GetDocument<Problem>(id);

            return ShowProblem(problem);
        }

        private IActionResult ShowProblem(Problem problem)
        {
            if (problem == null) throw new Exception("Problem does not exist!");
            var author = DocumentDbService.GetDocument<User>(problem.Author);
            var language = DocumentDbService.GetDocument<Language>(problem.Language, true);
            
            return View("Index", new ProblemDetails(problem, author, language, HttpContext.User.Identity.IsAuthenticated,
                HttpContext.User.Identity.Name));
        }

        public async Task<IEnumerable<SolutionDetail>> Solution(Guid id)
        {
            var currentUser = await GetRequestUser();

            var solutions = DocumentDbService.Client.CreateDocumentQuery<Solution>(DocumentDbService.DatabaseUri).Where(m => m.Type == DocumentType.Solution && m.Problem == id);
            
            var solutionDetails = new List<SolutionDetail>();
            foreach (var solution in solutions)
            {
                var currentUserName = currentUser?.Identity;
                var user = DocumentDbService.GetDocument<User>(solution.Author);
                var userVm = new UserViewModel(user, currentUserName, Url);
                var svm = new SolutionDetail(solution, userVm, Url);

                solutionDetails.Add(svm);
            }

            return solutionDetails.OrderByDescending(m => m.Votes).ThenBy(m => m.Passing != null && m.Passing.Value).ThenBy(m => m.Length).ToList();
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var problem = DocumentDbService.GetDocument<Problem>(id);
            var user = await GetRequestUser();

            if (user.Id != problem.Author)
                throw new Exception("User is not author!");

            var editProblem = new EditProblem(id, HttpContext.User.Identity.IsAuthenticated, HttpContext.User.Identity.Name);

            return View(editProblem);
        }

        [Authorize]
        [HttpPost]
        [Route("/problem/{id}")]
        public async Task<string> EditAsync(Guid id, Problem problem)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));

            if (string.IsNullOrEmpty(problem.Description) || string.IsNullOrWhiteSpace(problem.Name) || !problem.TestCases.Any())
                throw new Exception("All details are required to create a problem");

            var user = await GetRequestUser();

            var existingProblem = DocumentDbService.GetDocument<Problem>(problem.Id);

            if (existingProblem == null)
                throw new ArgumentNullException(nameof(problem));

            if (user.Id != existingProblem.Author)
                throw new Exception("User is not author!");

            existingProblem.TestCases = problem.TestCases;
            existingProblem.Description = problem.Description;
            existingProblem.Language = problem.Language;
            existingProblem.Name = problem.Name;

            await DocumentDbService.UpdateDocument(existingProblem);

            return Url.Action("Index", new {problem.Id});
        }


        [Authorize]
        [HttpPost]
        [Route("/problem/")]
        public async Task<string> PostAsync(Problem problem)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));

            if (string.IsNullOrEmpty(problem.Description) || string.IsNullOrWhiteSpace(problem.Name)  || !problem.TestCases.Any())
                throw new Exception("All details are required to create a problem");

            var currentUser = await GetRequestUser();
            problem.Author = currentUser.Id;

            await DocumentDbService.CreateDocument(problem);
            return Url.Action("Single", "Problem", new {problemName = problem.Name});
        }

        [HttpGet]
        public Language Language(Guid id)
        {
            var problem = DocumentDbService.Client.CreateDocumentQuery<Problem>(DocumentDbService.DatabaseUri)
                .FirstOrDefault(m => m.Id == id);

            return DocumentDbService.GetDocument<Language>(problem.Id, true);
        }

        [Authorize]
        public IActionResult New()
        {
            var user = GetRequestUser().Result;
            var viewModel = new NewProblem(true, user.Identity);
           
            return View(viewModel);
        }

        [HttpGet]
        public IEnumerable<Problem> Popular()
        {
            var list = DocumentDbService.Client.CreateDocumentQuery<Problem>(DocumentDbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.Problem && m.Solutions.Length > 5)
                .OrderByDescending(m => m.DateAdded)
                .ThenByDescending(m => m.Solutions.Length)
                .Take(10).ToList();

            return list;
        }

        public IActionResult Search( string criteria)
        {
            var recentProblems = SearchProblems(criteria);
            var vm = new SearchViewModel(criteria, recentProblems, HttpContext.User.Identity.IsAuthenticated, HttpContext.User.Identity.Name);

            return View(vm);
        }

        private IEnumerable<RecentProblem> SearchProblems(string critieria)
        {
            var problems = DocumentDbService.Client.CreateDocumentQuery<Problem>(DocumentDbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.Problem && (m.Name.ToLower().Contains(critieria.ToLower()) || m.Description.ToLower().Contains(critieria.ToLower())))
                .OrderByDescending(m => m.DateAdded)
                .Take(10).ToList();

            foreach (var problem in problems)
            {
                var language = DocumentDbService.GetDocument<Language>(problem.Language, true);
                var author = DocumentDbService.GetDocument<User>(problem.Author);

                if (language == null)
                    throw new Exception("Language cannot be null.");

                var solutions = DocumentDbService.Client.CreateDocumentQuery<Solution>(DocumentDbService.DatabaseUri)
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
            return DocumentDbService.GetDocument<Problem>(id);
        }
    }
}
