using System.Collections.Generic;
using System.Linq;
using CodeGolf.Models;
using CodeGolf.Sql.Models;

namespace CodeGolf.ViewModels
{
    public class ProblemDetails : AuthenticatedViewModel
    {
        private readonly Problem _problem;

        public ProblemDetails(Problem problem, User author, ICodeGolfLanguage language, bool authenticated, string identity) : base(authenticated, identity)
        {
            _problem = problem;
            Author = author.Identity;
            AuthorId = author.UserId.ToString();
            Language = language;
            IsAuthor = Author == identity;

            //TODO: Let's make this a bit more elegant...
            if (language.Name == "csharp")
            {
                SolutionHelp = Resource.CSharpHelp;
            }
        }

        public string Id => _problem.ProblemId.ToString();
        public string Name => _problem.Name;
        public string Description => _problem.Description;
        public IEnumerable<TestCaseViewModel> TestCases => _problem.TestCases.Select(m => new TestCaseViewModel(m));
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public bool IsAuthor { get; set; }
        public bool EnforceOutput => _problem.EnforceOutput;
        public ICodeGolfLanguage Language { get; set; }
        public string LanguageName { get; set; }
        public string SolutionHelp { get; set; }
        public bool Closed => _problem.Closed;
        public bool AnyLanguage => _problem.AnyLanguage;
    }

    public class TestCaseViewModel
    {
        private readonly TestCase _testCase;

        public TestCaseViewModel(TestCase testCase)
        {
            _testCase = testCase;
        }

        public string Input => _testCase.Input;

        public string Output => _testCase.Output;
    }
}
