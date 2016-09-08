using System.Collections.Generic;
using CodeGolf.Models;

namespace CodeGolf.ViewModels
{
    public class ProfileViewModel : AuthenticatedViewModel
    {
        public ProfileViewModel(bool authenticated, string identity = null) : base(authenticated, identity)
        {
        }

        public User User { get; set; }

        public IEnumerable<ProblemProfile> Problems { get; set; }
        public IEnumerable<SolutionProfile> Solutions { get; set; }

        public class ProblemProfile
        {
            public string Id { get; set; }
            public string Language { get; set; }
            public int Solutions { get; set; }
            public string Name { get; set; }
        }

        public class SolutionProfile
        {
            public string Id { get; set; }
            public string Language { get; set; }
            public string Problem { get; set; }
            public string ProblemId { get; set; }
            public int Length { get; set; }
            public bool IsTop { get; set; }
        }
    }
}
