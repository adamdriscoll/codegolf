using System;
using System.Collections.Generic;
using CodeGolf.Models;

namespace CodeGolf.ViewModels
{
    public class ProblemDetails : AuthenticatedViewModel
    {
        private readonly Problem _problem;

        public ProblemDetails(Problem problem, IEnumerable<SolutionDetail> solutions, string author, Language language, bool authenticated, string identity) : base(authenticated, identity)
        {
            _problem = problem;
            Solutions = solutions;
            Author = author;
            Language = language;
        }

        public string Id => _problem.Id.ToString();
        public string Name => _problem.Name;
        public string Description => _problem.Description;
        public string Input => _problem.Input;
        public string Output => _problem.Output;
        public string Author { get; set; }

        public IEnumerable<SolutionDetail> Solutions { get; set; }

        public Language Language { get; set; }
    }
}
