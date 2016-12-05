using System;
using CodeGolf.Models;

namespace CodeGolf.ViewModels
{
    public class SolutionCommentViewModel
    {
        private readonly Sql.Models.SolutionComment _comment;

        public SolutionCommentViewModel(Sql.Models.SolutionComment comment, string currentUser)
        {
            _comment = comment;

            DeleteCommentUrl = $"/solution/comment/{comment.SolutionCommentId}";

            Commentor = new UserViewModel(comment.Commentor, currentUser);
        }

        public int CommentId => _comment.SolutionCommentId;

        public string Comment => _comment.Comment;

        public UserViewModel Commentor { get;  }

        public string DeleteCommentUrl { get; }
    }
}
