using System;

namespace CodeGolf.ViewModels
{
    public class EditProblem : AuthenticatedViewModel
    {
        public EditProblem(int problem, bool authenticated, string identity = null) : base(authenticated, identity)
        {
            ProblemId = problem;
        }

        public int ProblemId { get; set; }
    }
}
