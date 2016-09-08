using System;
using CodeGolf.Models;

namespace CodeGolf.ViewModels
{
    public class SolutionDetail
    {
        private readonly Solution _solution;

        public SolutionDetail(Solution solution, User author)
        {
            _solution = solution;
            Author = author.Identity;
            AuthorId = author.Id.ToString();
        }

        public string Content => _solution.Content;
        public Guid Id => _solution.Id;
        public int Length => _solution.Length;
        public DateTime RoundPlayed => _solution.DateAdded;
        public string Author { get; set; }
        public string AuthorId { get; set; }

        public bool? Passing => _solution.Passing;
    }
}
