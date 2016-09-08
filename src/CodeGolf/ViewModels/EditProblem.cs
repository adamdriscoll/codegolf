using System.Collections.Generic;
using CodeGolf.Models;

namespace CodeGolf.ViewModels
{
    public class EditProblem : AuthenticatedViewModel
    {
        public EditProblem(Problem problem, bool authenticated, string identity = null) : base(authenticated, identity)
        {
            Problem = problem;
        }

        public Problem Problem { get; set; }

        public IEnumerable<Language> Languages { get; set; }
    }
}
