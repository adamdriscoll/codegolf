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
    public class ProfileController : Controller
    {
        private DocumentDbService _documentDbService;

        public ProfileController(DocumentDbService documentDbService)
        {
            _documentDbService = documentDbService;
        }

        public IActionResult Get(Guid? id)
        {
            var profileViewModel = new ProfileViewModel(this.HttpContext.User.Identity.IsAuthenticated, this.HttpContext.User.Identity.Name);

            if (id.HasValue)
            {
                profileViewModel.User = _documentDbService.GetDocument<User>(id.Value);
                if (profileViewModel.User == null)
                {
                    return NotFound();
                }
            }
            else if (this.HttpContext.User.Identity.IsAuthenticated)
            {
                profileViewModel.User = _documentDbService.Client.CreateDocumentQuery<User>(_documentDbService.DatabaseUri).Where(m => m.Identity == this.HttpContext.User.Identity.Name && m.Authentication == this.HttpContext.User.Identity.AuthenticationType).ToList().FirstOrDefault();
            }
            else
            {
                return NotFound();
            }
            
            var problemProfiles = new List<ProfileViewModel.ProblemProfile>();
            foreach (
                var problem in
                    _documentDbService.Client.CreateDocumentQuery<Problem>(_documentDbService.DatabaseUri)
                        .Where(m => m.Author == profileViewModel.User.Id && m.Type == DocumentType.Problem))
            {
                var language = _documentDbService.GetDocument<Language>(problem.Language, true);
                problemProfiles.Add(new ProfileViewModel.ProblemProfile
                {
                    Id = problem.Id.ToString(),
                    Language = language.DisplayName,
                    Solutions = problem.SolutionCount,
                    Name = problem.Name
                });
            }

            profileViewModel.Problems = problemProfiles;

            var solutionProfiles = new List<ProfileViewModel.SolutionProfile>();
            var solutions = _documentDbService.Client.CreateDocumentQuery<Solution>(_documentDbService.DatabaseUri).Where(m => m.Author == profileViewModel.User.Id && m.Type == DocumentType.Solution);
            foreach (var solution in solutions)
            {
                var problem = _documentDbService.GetDocument<Problem>(solution.Problem);

                solutionProfiles.Add(new ProfileViewModel.SolutionProfile
                {
                    Id = solution.Id.ToString(),
                    Problem = problem.Name,
                    ProblemId = problem.Id.ToString(),
                    Length = solution.Length,
                    Language = _documentDbService.GetDocument<Language>(problem.Language, true).DisplayName

                });
            }

            profileViewModel.Solutions = solutionProfiles;

            return View("Index", profileViewModel);
        }
    }
}
