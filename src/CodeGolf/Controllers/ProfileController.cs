using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Sql.Repository;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class ProfileController : AuthorizedController
    {
        private readonly IRepository _repository;

        public ProfileController(IRepository repository) : base(repository)
        {
            _repository = repository;
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
                var profiles = _repository.Users.Find(profile).ToList();
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
            var problems = _repository.Problem.GetByUser(profileViewModel.User);
            foreach (var problem in problems)
            {
                problemProfiles.Add(new ProfileViewModel.ProblemProfile
                {
                    Id = problem.ProblemId.ToString(),
                    Language = problem.Language,
                    Solutions = problem.Solutions.Count,
                    Name = problem.Name
                });
            }

            profileViewModel.Problems = problemProfiles;

            var solutionProfiles = new List<ProfileViewModel.SolutionProfile>();
            var solutions = profileViewModel.User.Solutions;
            foreach (var solution in solutions)
            {
                if (!solution.ProblemId.HasValue) { continue;}

                var problem = await _repository.Problem.Get(solution.ProblemId.Value);

                solutionProfiles.Add(new ProfileViewModel.SolutionProfile
                {
                    Id = solution.SolutionId.ToString(),
                    Problem = problem.Name,
                    ProblemId = problem.ProblemId.ToString(),
                    Length = solution.Content.Length,
                    Language = problem.Language

                });
            }

            profileViewModel.Solutions = solutionProfiles;

            return View("Index", profileViewModel);
        }
    }
}
