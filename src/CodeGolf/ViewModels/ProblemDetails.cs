using System.Collections.Generic;
using CodeGolf.Models;

namespace CodeGolf.ViewModels
{
    public class ProblemDetails : AuthenticatedViewModel
    {
        private readonly Problem _problem;

        public ProblemDetails(Problem problem, User author, Language language, bool authenticated, string identity) : base(authenticated, identity)
        {
            _problem = problem;
            Author = author.Identity;
            AuthorId = author.Id.ToString();
            Language = language;
            IsAuthor = Author == identity;

            //TODO: Let's make this a bit more elegant...
            if (language.Name == "csharp")
            {
                SolutionHelp = Resource.CSharpHelp;
            }
        }

        public string Id => _problem.Id.ToString();
        public string Name => _problem.Name;
        public string Description => _problem.Description;
        public IEnumerable<Problem.TestCase> TestCases => _problem.TestCases;
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public bool IsAuthor { get; set; }
        public bool EnforceOutput => _problem.EnforceOutput;
        public Language Language { get; set; }
        public string SolutionHelp { get; set; }
        public bool Closed => _problem.Closed;
    }
}
