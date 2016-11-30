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
    public class ProfileController : AuthorizedController
    {
        private readonly DocumentDbService _documentDbService;

        public ProfileController(DocumentDbService documentDbService) : base(documentDbService)
        {
            _documentDbService = documentDbService;
        }

        [Route("/profile/")]
        public async Task<IActionResult> Mine()
        {
            var profileViewModel = new ProfileViewModel(this.HttpContext.User.Identity.IsAuthenticated, this.HttpContext.User.Identity.Name);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                profileViewModel.User = await GetRequestUser();
            }

            return await ReturnProfileView(profileViewModel);
        }

        [Route("/profile/{profile}")]
        public async Task<IActionResult> Find(string profile)
        {
            var profileViewModel = new ProfileViewModel(this.HttpContext.User.Identity.IsAuthenticated, this.HttpContext.User.Identity.Name);
            if (string.IsNullOrEmpty(profile) && !HttpContext.User.Identity.IsAuthenticated)
            {
                return NotFound();
            }
            else if (string.IsNullOrEmpty(profile) && HttpContext.User.Identity.IsAuthenticated)
            {
                profileViewModel.User = await GetRequestUser();
            }
            else if (!string.IsNullOrEmpty(profile))
            {
                var profiles = await _documentDbService.Repository.Users.Find(profile);
                if (profiles.Count() > 1)
                {
                    throw new Exception("Expected one profile.");
                }

                profileViewModel.User = profiles.First();
            }

            return await ReturnProfileView(profileViewModel);
        }

        private async Task<IActionResult> ReturnProfileView(ProfileViewModel profileViewModel)
        {
            var problemProfiles = new List<ProfileViewModel.ProblemProfile>();
            var problems = await _documentDbService.Repository.Problem.GetByUser(profileViewModel.User);
            foreach (var problem in problems)
            {
                problemProfiles.Add(new ProfileViewModel.ProblemProfile
                {
                    Id = problem.Id.ToString(),
                    Language = problem.LanguageName,
                    Solutions = problem.SolutionCount,
                    Name = problem.Name
                });
            }

            profileViewModel.Problems = problemProfiles;

            var solutionProfiles = new List<ProfileViewModel.SolutionProfile>();
            var solutions = _documentDbService.Client.CreateDocumentQuery<Solution>(_documentDbService.DatabaseUri).Where(m => m.Author == profileViewModel.User.Id && m.Type == DocumentType.Solution);
            foreach (var solution in solutions)
            {
                var problem = await _documentDbService.Repository.Problem.Get(solution.Problem);

                solutionProfiles.Add(new ProfileViewModel.SolutionProfile
                {
                    Id = solution.Id.ToString(),
                    Problem = problem.Name,
                    ProblemId = problem.Id.ToString(),
                    Length = solution.Length,
                    Language = problem.LanguageName

                });
            }

            profileViewModel.Solutions = solutionProfiles;

            return View("Index", profileViewModel);
        }
    }
}
