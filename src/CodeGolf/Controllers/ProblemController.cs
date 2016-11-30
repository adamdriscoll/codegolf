using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Models;
using CodeGolf.Services;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class ProblemController : AuthorizedController
    {
        private readonly LanguageFactory _languageFactory;
        public ProblemController(DocumentDbService dbService, LanguageFactory factory) : base(dbService)
        {
            _languageFactory = factory;
        }

        public async Task<IActionResult> Single(string problemName)
        {
            var problem = await DocumentDbService.Repository.Problem.Get(problemName);

            return await ShowProblem(problem);
        }

        public async Task<IActionResult> Index(Guid id)
        {
            var problem = await DocumentDbService.Repository.Problem.Get(id);

            return await ShowProblem(problem);
        }

        private async Task<IActionResult> ShowProblem(Problem problem)
        {
            if (problem == null) throw new Exception("Problem does not exist!");
            var author = await DocumentDbService.Repository.Users.Get(problem.Author);

            var language = _languageFactory.Get(problem.LanguageName);

            return View("Index", new ProblemDetails(problem, author, language, HttpContext.User.Identity.IsAuthenticated, HttpContext.User.Identity.Name));
        }

        public async Task<IEnumerable<SolutionDetail>> Solution(Guid id)
        {
            var currentUser = await GetRequestUser();

            var solutions = DocumentDbService.Client.CreateDocumentQuery<Solution>(DocumentDbService.DatabaseUri).Where(m => m.Type == DocumentType.Solution && m.Problem == id);
            
            var solutionDetails = new List<SolutionDetail>();
            foreach (var solution in solutions)
            {
                var currentUserName = currentUser?.Identity;
                var user = await DocumentDbService.Repository.Users.Get(solution.Author);
                var userVm = new UserViewModel(user, currentUserName);
                var svm = new SolutionDetail(solution, userVm, Url);

                solutionDetails.Add(svm);
            }

            return solutionDetails.OrderByDescending(m => m.Votes).ThenBy(m => m.Passing != null && m.Passing.Value).ThenBy(m => m.Length).ToList();
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var problem = await DocumentDbService.Repository.Problem.Get(id);
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

            var existingProblem = await DocumentDbService.Repository.Problem.Get(id);

            if (existingProblem == null)
                throw new ArgumentNullException(nameof(problem));

            if (user.Id != existingProblem.Author)
                throw new Exception("User is not author!");

            existingProblem.TestCases = problem.TestCases;
            existingProblem.Description = problem.Description;
            existingProblem.LanguageName = problem.LanguageName;
            existingProblem.Name = problem.Name;

            await DocumentDbService.Repository.Problem.Update(existingProblem);

            return Url.Action("Index", new {problem.Id});
        }


        [Authorize]
        [HttpPost]
        [Route("/problem/{id}/close")]
        public async Task Close(Guid id)
        {
            var problem = await DocumentDbService.Repository.Problem.Get(id);
            var user = await GetRequestUser();

            if (problem.Author != user.Id)
            {
                throw new Exception("Current user is not author of problem!");
            }

            await DocumentDbService.Repository.Problem.Close(problem);
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

            problem.Id = Guid.NewGuid();
            problem.Author = currentUser.Id;
            problem.AuthorModel = currentUser;

            await DocumentDbService.Repository.Problem.Create(problem);

            return Url.Action("Single", "Problem", new {problemName = problem.Name});
        }

        [HttpGet]
        public async Task<ICodeGolfLanguage> Language(Guid id)
        {
            var problem = await DocumentDbService.Repository.Problem.Get(id);

            return _languageFactory.Get(problem.LanguageName);
        }

        [Authorize]
        public async Task<IActionResult> New()
        {
            var user = await GetRequestUser();
            var viewModel = new NewProblem(true, user.Identity);
           
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IEnumerable<Problem>> Popular()
        {
            return await DocumentDbService.Repository.Problem.GetPopularProblems();
        }

        public async Task<IActionResult> Search( string criteria)
        {
            var recentProblems = await SearchProblems(criteria);
            var vm = new SearchViewModel(criteria, recentProblems, HttpContext.User.Identity.IsAuthenticated, HttpContext.User.Identity.Name);

            return View(vm);
        }

        private async Task<IEnumerable<RecentProblem>> SearchProblems(string critieria)
        {
            var recentProblems = new List<RecentProblem>();

            var problems = await DocumentDbService.Repository.Problem.Find(critieria);

            foreach (var problem in problems)
            {
                var solutions = DocumentDbService.Client.CreateDocumentQuery<Solution>(DocumentDbService.DatabaseUri)
                    .Where(m => problem.Solutions.Contains(m.Id))
                    .OrderBy(m => m.Length)
                    .ToList();

                var topSolution = solutions.FirstOrDefault();

                var topSolutionLength = 0;
                if (topSolution != null)
                    topSolutionLength = topSolution.Length;

                recentProblems.Add(new RecentProblem
                {
                    Name = problem.Name,
                    Id = problem.Id,
                    Language = problem.LanguageName,
                    ShortestSolution = topSolutionLength,
                    SolutionCount = solutions.Count,
                    Author = problem.AuthorModel.Identity
                });
            }

            return recentProblems;
        }

        [HttpGet]
        public async Task<Problem> Get(Guid id)
        {
            return await DocumentDbService.Repository.Problem.Get(id);
        }
    }
}
