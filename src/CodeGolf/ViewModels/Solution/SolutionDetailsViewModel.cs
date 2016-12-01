using System.Collections.Generic;
using CodeGolf.Models;

namespace CodeGolf.ViewModels
{
    public class SolutionDetailsViewModel
    {
        public string Content { get; set; }
        public string Langauge { get; set; }
        public string UpvoteUrl { get; set; }
        public string DownvoteUrl { get; set; }
        public string AddCommentUrl { get; set; }
        public int Votes { get; set; }
        public IEnumerable<SolutionCommentViewModel> Comments { get; set; }
        public string Language { get; set; }

    }
}
