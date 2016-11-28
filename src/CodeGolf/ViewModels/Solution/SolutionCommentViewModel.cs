using System;
using CodeGolf.Models;

namespace CodeGolf.ViewModels
{
    public class SolutionCommentViewModel
    {
        private readonly SolutionComment _comment;

        public SolutionCommentViewModel(SolutionComment comment, string currentUser)
        {
            _comment = comment;

            DeleteCommentUrl = $"/solution/comment/{comment.Id}";

            Commentor = new UserViewModel(comment.Commentor, currentUser);
        }

        public Guid CommentId => _comment.Id;

        public string Comment => _comment.Comment;

        public UserViewModel Commentor { get;  }

        public string DeleteCommentUrl { get; }
    }
}
