using System.Collections.Generic;

namespace CodeGolf.ViewModels
{
    public class HomeViewModel : AuthenticatedViewModel
    {
        public HomeViewModel(IEnumerable<RecentProblem> recentProblems, IEnumerable<RecentProblem> popularProblems, bool authenticated, string identity = null) : base(authenticated, identity)
        {
            RecentProblems = recentProblems;
            PopularProblems = popularProblems;
        }

        public IEnumerable<RecentProblem> RecentProblems { get; private set; }

        public IEnumerable<RecentProblem> PopularProblems { get; private set; }
    }
}
