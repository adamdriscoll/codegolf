using System.Collections.Generic;
using CodeGolf.Models;

namespace CodeGolf.ViewModels
{
    public class NewProblem : AuthenticatedViewModel
    {
        public NewProblem(bool authenticated, string identity = null) : base(authenticated, identity)
        {
        }

        public IEnumerable<Language> Languages { get; set; }
    }
}
