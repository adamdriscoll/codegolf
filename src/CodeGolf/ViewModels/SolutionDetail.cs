using System;
using CodeGolf.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CodeGolf.ViewModels
{
    public class SolutionDetail
    {
        private readonly Solution _solution;

        public SolutionDetail(Solution solution, UserViewModel author, IUrlHelper urlHelper)
        {
            _solution = solution;
            Author = author;

            DeleteSolutionUrl = urlHelper.Action(new UrlActionContext
            {
                Action = "DeleteAsync",
                Controller = "Solution",
                Values = new {guid = solution.Id}
            });

            UpvoteUrl = urlHelper.Action(new UrlActionContext
            {
                Action = "Upvote",
                Controller = "Solution",
                Values = new
                {
                    itemId = Id
                }
            });

            DownvoteUrl = urlHelper.Action(new UrlActionContext
            {
                Action = "Downvote",
                Controller = "Solution",
                Values = new
                {
                    itemId = Id
                }
            });

            ContentUrl = urlHelper.Action(new UrlActionContext
            {
                Action = "Details",
                Controller = "Solution",
                Values = new
                {
                    id = Id
                }
            });
        }

        public Guid Id => _solution.Id;
        public int Length => _solution.Length;
        public DateTime Date => _solution.DateAdded;

        public string DeleteSolutionUrl { get; set; }

        public string UpvoteUrl { get; set; }

        public string DownvoteUrl { get; set; }

        public string ContentUrl { get; set; }

        public int Votes => _solution.Votes;

        public UserViewModel Author { get; set; }

        public bool? Passing => _solution.Passing;
    }
}
