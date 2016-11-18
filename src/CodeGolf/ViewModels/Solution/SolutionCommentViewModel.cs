using System;
using CodeGolf.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CodeGolf.ViewModels
{
    public class SolutionCommentViewModel
    {
        private readonly SolutionComment _comment;

        public SolutionCommentViewModel(SolutionComment comment, string currentUser, IUrlHelper urlHelper)
        {
            _comment = comment;

            DeleteCommentUrl = $"/solution/comment/{comment.Id}";

            Commentor = new UserViewModel(comment.Commentor, currentUser, urlHelper);
        }

        public Guid CommentId => _comment.Id;

        public string Comment => _comment.Comment;

        public UserViewModel Commentor { get;  }

        public string DeleteCommentUrl { get; }
    }
}
