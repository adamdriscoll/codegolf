using System.Collections.Generic;

namespace CodeGolf.ViewModels
{
    public class SearchViewModel : AuthenticatedViewModel
    {
        public SearchViewModel(string criteria, IEnumerable<RecentProblem> problems, bool authenticated, string identity = null) : base(authenticated, identity)
        {
            Problems = problems;
            Criteria = criteria;
        }

        public IEnumerable<RecentProblem> Problems { get; private set; }

        public string Criteria { get; set; }
    }
}
