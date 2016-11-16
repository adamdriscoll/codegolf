using System;

namespace CodeGolf.ViewModels
{
    public class EditProblem : AuthenticatedViewModel
    {
        public EditProblem(Guid problem, bool authenticated, string identity = null) : base(authenticated, identity)
        {
            ProblemId = problem;
        }

        public Guid ProblemId { get; set; }
    }
}
