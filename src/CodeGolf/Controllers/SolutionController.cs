using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.Interfaces;
using CodeGolf.Services;
using CodeGolf.Sql.Repository;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class SolutionController : AuthorizedController
    {
        private readonly ProblemValidatorService _problemValidatorService;

        public SolutionController(IRepository repository, ProblemValidatorService problemValidatorService) : base(repository)
        {
            _problemValidatorService = problemValidatorService;
        }

        [HttpGet]
        public async Task<Sql.Models.Solution> Get(int id)
        {
            return await Repository.Solutions.Get(id);
        }

        [HttpGet]
        public async Task<string> Raw(int id)
        {
            return (await Get(id)).Content;
        }

        [Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var solution = await Get(id);
            if (solution == null)
                throw new Exception("Solution not found!");

            var user = await GetRequestUser();

            if (solution.Author.UserId != user.UserId)
                throw new Exception("User does not own solution!");

            var problem = solution.Problem;
            if (problem == null)
                throw new Exception("Problem not found!");

            await Repository.Votes.DeleteForItemId(solution.SolutionId);
            await Repository.Solutions.Delete(solution);

            return Redirect("/Problem/Index/" + problem.ProblemId);
        }

        [Authorize]
        public async Task<ValidationResult> ValidateAsync(int problem, string content)
        {
            var theProblem = await Repository.Problem.Get(problem);
            return await _problemValidatorService.Validate(theProblem.Language, theProblem, content);
        } 

        [Authorize]
        public async Task<IActionResult> PostAsync(Sql.Models.Solution solution)
        {
            if (solution == null)
                throw new ArgumentNullException(nameof(solution));

            if (string.IsNullOrWhiteSpace(solution.Content))
                throw new Exception("Content is required for solution.");

            var user = await GetRequestUser();
            solution.Author = user;

            await Repository.Solutions.Create(solution);

            return Redirect("/Problem/Index/" + solution.ProblemId);
        }

        [Route("solution/{id}/details")]
        public async Task<SolutionDetailsViewModel> Details(int id)
        {
            var currentUser = await GetRequestUser();
            var currentUserName = currentUser?.Identity;

            var solution = await Get(id);
            if (solution == null)
                throw new Exception("Solution not found!");

            var comments = Repository.Comments.GetSolutionComments(id).ToList();

            var votes = await Repository.Votes.GetCountForItemId(id);

            return new SolutionDetailsViewModel
            {
                Content = solution.Content,
                UpvoteUrl = Url.Action("Upvote", new {itemId = id}),
                DownvoteUrl = Url.Action("Downvote", new { itemId = id }),
                AddCommentUrl = Url.Action("AddComment", new { id }),
                Comments = comments.Select(m => new SolutionCommentViewModel(m, currentUserName)),
                Language = solution.Language,
                Votes = votes
                //TODO: Langauge = solution.Language.Name
            };
        }

        [Authorize]
        [Route("solution/{itemId}/upvote")]
        public async Task<int> Upvote(int itemId)
        {
            return await Vote(itemId, true);
        }

        [Authorize]
        [Route("solution/{itemId}/downvote")]
        public async Task<int> Downvote(int itemId)
        {
            return await Vote(itemId, false);
        }

        [Route("solution/{id}/comment")]
        public async  Task<IEnumerable<SolutionCommentViewModel>> Comments(int id)
        {
            var currentUser = await GetRequestUser();
            var currentUserName = currentUser?.Identity;

            return
                Repository.Comments.GetSolutionComments(id)
                    .Select(m => new SolutionCommentViewModel(m, currentUserName));
        }

        [Authorize]
        [Route("solution/comment/{id}")]
        [HttpDelete]
        public async Task DeleteComment(int id)
        {
            var currentUser = await GetRequestUser();
            var comment = await Repository.Comments.GetSolutionComment(id);

            if (currentUser.UserId != comment.Commentor.UserId)
            {
                throw new Exception("User is not commentor and cannot delete comment!");
            }

            await Repository.Comments.DeleteSolutionComment(id);
        }

        [Authorize]
        [Route("solution/{id}/comment")]
        [HttpPost]
        public async Task<SolutionCommentViewModel> AddComment(int id, string comment)
        {
            var currentUser = await GetRequestUser();
            var solutionComment = new Sql.Models.SolutionComment();
            solutionComment.SolutionId = id;
            solutionComment.Comment = comment;
            solutionComment.Commentor = currentUser;
            solutionComment = await Repository.Comments.AddSolutionComment(solutionComment);

            return new SolutionCommentViewModel(solutionComment, currentUser.Identity);
        }

        private async Task<int> Vote(int itemId, bool upvote)
        {
            var value = upvote ? 1 : -1;

            var user = await GetRequestUser();

            var solution = await Get(itemId);

            var castVote = await Repository.Votes.GetVoteByItemIdAndUser(itemId, user.UserId);

            var vote = new Sql.Models.Vote();
            vote.ItemId = itemId;
            vote.Value = value;

            if (castVote == null)
            {
                vote.Voter = user;

                await Repository.Votes.Create(vote);
                await Repository.SaveChangesAsync();
            }
            else if (castVote.Value != vote.Value)
            {
                castVote.Value = vote.Value;
                await Repository.SaveChangesAsync();
            }

            return await Repository.Votes.GetCountForItemId(solution.SolutionId);
        }
    }
}
