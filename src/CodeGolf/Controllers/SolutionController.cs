using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGolf.Models;
using CodeGolf.Services;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class SolutionController : AuthorizedController
    {
        private readonly ProblemValidatorService _problemValidatorService;

        public SolutionController(DocumentDbService dbService, ProblemValidatorService problemValidatorService) : base(dbService)
        {
            _problemValidatorService = problemValidatorService;
        }

        [HttpGet]
        public Solution Get(Guid id)
        {
            return DocumentDbService.GetDocument<Solution>(id);
        }

        [HttpGet]
        public string Raw(Guid id)
        {
            return DocumentDbService.GetDocument<Solution>(id).Content;
        }

        [Authorize]
        public async Task<IActionResult> DeleteAsync(Guid guid)
        {
            var solution = DocumentDbService.GetDocument<Solution>(guid);
            if (solution == null)
                throw new Exception("Solution not found!");

            var user = await GetRequestUser();

            if (solution.Author != user.Id)
                throw new Exception("User does not own solution!");

            var problem = DocumentDbService.GetDocument<Problem>(solution.Problem);
            if (problem == null)
                throw new Exception("Problem not found!");

            var solutions = problem.Solutions.ToList();
            solutions.Remove(guid);
            problem.Solutions = solutions.ToArray();
            problem.SolutionCount--;
            await DocumentDbService.UpdateDocument(problem);
            await DocumentDbService.DeleteDocument(solution.Id);

            return Redirect("/Problem/Index/" + problem.Id);
        }

        [Authorize]
        public async Task<ValidationResult> ValidateAsync(Guid problem, string content)
        {
            var theProblem = DocumentDbService.GetDocument<Problem>(problem);
            var language = DocumentDbService.GetDocument<Language>(theProblem.Language, true);

            return await _problemValidatorService.Validate(language.Name, theProblem, content);
        } 

        [Authorize]
        public async Task<IActionResult> PostAsync(Solution solution)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            if (string.IsNullOrWhiteSpace(solution.Content))
                throw new Exception("Content is required for solution.");

            var user = await GetRequestUser();
            solution.Author = user.Id;

            await DocumentDbService.CreateDocument(solution);
            var problem = DocumentDbService.GetDocument<Problem>(solution.Problem);
            var list = problem.Solutions.ToList();
            list.Add(solution.Id);
            problem.Solutions = list.ToArray();
            problem.SolutionCount = problem.Solutions.Length;

            await DocumentDbService.Client.UpsertDocumentAsync(DocumentDbService.DatabaseUri, problem);

            return Redirect("/Problem/Index/" + problem.Id);
        }

        [Authorize]
        public async Task<int> Vote(Vote vote)
        {
            if (vote.Value != 1 && vote.Value != -1)
            {
                throw new Exception("Invalid vote value!");
            }

            var user = await GetRequestUser();

            var solution = DocumentDbService.GetDocument<Solution>(vote.Item);

            var castVote = DocumentDbService.GetDocumentType<Vote>(DocumentType.Vote)
                .Where(m => m.Item == vote.Item && m.Voter == user.Id)
                .ToList()
                .FirstOrDefault();

            if (castVote == null)
            {
                vote.Voter = user.Id;
                vote.ItemType = DocumentType.Solution;
                await DocumentDbService.CreateDocument(vote);
                solution.Votes += vote.Value;
                await DocumentDbService.UpdateDocument(solution);
            }
            else if (castVote.Value != vote.Value)
            {
                castVote.Value = vote.Value;
                await DocumentDbService.UpdateDocument(castVote);
                solution.Votes += vote.Value * 2;
                await DocumentDbService.UpdateDocument(solution);
            }

            return solution.Votes;
        }
    }
}
