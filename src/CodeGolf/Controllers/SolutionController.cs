using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
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

            var problem = await DocumentDbService.Repository.Problem.Get(solution.Problem);
            if (problem == null)
                throw new Exception("Problem not found!");

            var solutions = problem.Solutions.ToList();
            solutions.Remove(guid);
            problem.Solutions = solutions.ToArray();
            problem.SolutionCount--;

            await DocumentDbService.Repository.Problem.Update(problem);
            await DocumentDbService.DeleteDocument(solution.Id);

            return Redirect("/Problem/Index/" + problem.Id);
        }

        [Authorize]
        public async Task<ValidationResult> ValidateAsync(Guid problem, string content)
        {
            var theProblem = await DocumentDbService.Repository.Problem.Get(problem);
            return await _problemValidatorService.Validate(theProblem.LanguageModel.Name, theProblem, content);
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
            var problem = await DocumentDbService.Repository.Problem.Get(solution.Problem);

            var list = problem.Solutions.ToList();
            list.Add(solution.Id);
            problem.Solutions = list.ToArray();
            problem.SolutionCount = problem.Solutions.Length;
            await DocumentDbService.Repository.Problem.Update(problem);

            return Redirect("/Problem/Index/" + problem.Id);
        }

        [Route("solution/{id}/details")]
        public async Task<SolutionDetailsViewModel> Details(Guid id)
        {
            var currentUser = await GetRequestUser();
            var currentUserName = currentUser?.Identity;

            var solution = DocumentDbService.GetDocument<Solution>(id);
            if (solution == null)
                throw new Exception("Solution not found!");

            var comments = DocumentDbService.Repository.Comments.GetSolutionComments(id).ToList();

            return new SolutionDetailsViewModel
            {
                Content = solution.Content,
                UpvoteUrl = Url.Action("Upvote", new {itemId = id}),
                DownvoteUrl = Url.Action("Downvote", new { itemId = id }),
                AddCommentUrl = Url.Action("AddComment", new { id }),
                Comments = comments.Select(m => new SolutionCommentViewModel(m, currentUserName)),
                Votes = solution.Votes
                //TODO: Langauge = solution.Language.Name
            };
        }

        [Authorize]
        [Route("solution/{itemId}/upvote")]
        public async Task<int> Upvote(Guid itemId)
        {
            return await Vote(itemId, true);
        }

        [Authorize]
        [Route("solution/{itemId}/downvote")]
        public async Task<int> Downvote(Guid itemId)
        {
            return await Vote(itemId, false);
        }

        [Route("solution/{id}/comment")]
        public async  Task<IEnumerable<SolutionCommentViewModel>> Comments(Guid id)
        {
            var currentUser = await GetRequestUser();
            var currentUserName = currentUser?.Identity;

            return
                DocumentDbService.Repository.Comments.GetSolutionComments(id)
                    .Select(m => new SolutionCommentViewModel(m, currentUserName));
        }

        [Authorize]
        [Route("solution/comment/{id}")]
        [HttpDelete]
        public async Task DeleteComment(Guid id)
        {
            var currentUser = await GetRequestUser();
            var comment = DocumentDbService.Repository.Comments.GetSolutionComment(id);

            if (currentUser.Id != comment.Commentor.Id)
            {
                throw new Exception("User is not commentor and cannot delete comment!");
            }

            await DocumentDbService.Repository.Comments.DeleteSolutionComment(id);
        }

        [Authorize]
        [Route("solution/{id}/comment")]
        [HttpPost]
        public async Task<SolutionCommentViewModel> AddComment(Guid id, string comment)
        {
            var currentUser = await GetRequestUser();
            var solutionComment = new SolutionComment();
            solutionComment.Solution = id;
            solutionComment.Comment = comment;
            solutionComment.Commentor = currentUser;
            var commentId = await DocumentDbService.Repository.Comments.AddSolutionComment(solutionComment);
            solutionComment.Id = new Guid(commentId);

            return new SolutionCommentViewModel(solutionComment, currentUser.Identity);
        }

        private async Task<int> Vote(Guid itemId, bool upvote)
        {
            var value = upvote ? 1 : -1;

            var user = await GetRequestUser();

            var solution = DocumentDbService.GetDocument<Solution>(itemId);

            var castVote = DocumentDbService.GetDocumentType<Vote>(DocumentType.Vote)
                .Where(m => m.Item == itemId && m.Voter == user.Id)
                .ToList()
                .FirstOrDefault();

            var vote = new Vote();
            vote.Item = itemId;
            vote.Value = value;

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
