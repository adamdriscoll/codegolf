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
    public class ProblemController : AuthorizedController
    {
        public ProblemController(DocumentDbService dbService) : base(dbService)
        {
        }

        // GET: /<controller>/
        public IActionResult Index(Guid id)
        {
            var problem = DocumentDbService.GetDocument<Problem>(id);
            var author = DocumentDbService.GetDocument<User>(problem.Author);
            var language = DocumentDbService.GetDocument<Language>(problem.Language, true);

            if (problem == null) throw new Exception("Problem does not exist!");

            var solutions = DocumentDbService.Client.CreateDocumentQuery<Solution>(DocumentDbService.DatabaseUri)
                .Where(m => m.Type == DocumentType.Solution && problem.Solutions.Contains(m.Id));

            var solutionDetails = new List<SolutionDetail>();
            foreach (var solution in solutions)
            {
                var user = DocumentDbService.GetDocument<User>(solution.Author);

                var svm = new SolutionDetail(solution, user);

                solutionDetails.Add(svm);
            }

            solutionDetails = solutionDetails.OrderByDescending(m => m.Votes).ThenBy(m => m.Passing != null && m.Passing.Value).ThenBy(m => m.Length).ToList();

            return View(new ProblemDetails(problem, solutionDetails, author, language, HttpContext.User.Identity.IsAuthenticated,
                HttpContext.User.Identity.Name));
        }


        [Authorize]
        public IActionResult Edit(Guid id)
        {
            var problem = DocumentDbService.GetDocument<Problem>(id);

            var editProblem = new EditProblem(problem, HttpContext.User.Identity.IsAuthenticated, HttpContext.User.Identity.Name);
            editProblem.Languages = DocumentDbService.GetDocumentType<Language>(DocumentType.Language);

            return View(editProblem);
        }

        [Authorize]
        public async Task<IActionResult> EditAsync(Guid id, string input, string output, string description, Guid language, string name)
        {
            var user = await GetRequestUser();

            if (string.IsNullOrEmpty(output) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(name))
                throw new Exception("All details are required to create a problem");

            var problem = DocumentDbService.GetDocument<Problem>(id);

            if (problem == null)
                throw new ArgumentNullException(nameof(problem));

            if (user.Id != problem.Author)
                throw new Exception("User is not author!");

            problem.TestCases = new List<Problem.TestCase>
            {
                new Problem.TestCase
                {
                    Input = input,
                    Output = output
                }
            };
            
            problem.Description = description;
            problem.Language = language;
            problem.Name = name;

            await DocumentDbService.UpdateDocument(problem);

            return Redirect("/Problem/Index/" + problem.Id);
        }


        [Authorize]
        public async Task<IActionResult> PostAsync(Problem problem)
        {
            if (problem == null)
                throw new ArgumentNullException(nameof(problem));

            if (string.IsNullOrEmpty(problem.Description) || string.IsNullOrWhiteSpace(problem.Name)  || string.IsNullOrEmpty(problem.Output))
                throw new Exception("All details are required to create a problem");

            var user = DocumentDbService.Client.CreateDocumentQuery<User>(DocumentDbService.DatabaseUri).Where(m => m.Identity == this.HttpContext.User.Identity.Name && m.Authentication == this.HttpContext.User.Identity.AuthenticationType).ToList().FirstOrDefault();
            if (user == null)
            {
                user = new User
                {
                    Identity = this.HttpContext.User.Identity.Name,
                    Authentication = this.HttpContext.User.Identity.AuthenticationType
                };

                await DocumentDbService.CreateDocument(user);
            }

            problem.Author = user.Id;

            problem.TestCases = new List<Problem.TestCase>
            {
                new Problem.TestCase
                {
                    Input = problem.Input,
                    Output = problem.Output
                }
            };

            await DocumentDbService.CreateDocument(problem);

            return Redirect("/Problem/Index/" + problem.Id);
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
            var viewModel = new NewProblem(true, user.Identity)
            {
                Languages = DocumentDbService.Client.CreateDocumentQuery<Language>(DocumentDbService.DatabaseUri).Where(m => m.Type == DocumentType.Language).OrderBy(m => m.DisplayName)
            };
           
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
                .Where(m => m.Type == DocumentType.Problem && (m.Name.Contains(critieria) || m.Description.Contains(critieria)))
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
