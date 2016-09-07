using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeGolf.ViewModels
{
    public class AuthenticatedViewModel
    {
        public AuthenticatedViewModel(bool authenticated, string identity = null)
        {
            Authenticated = authenticated;
            Identity = identity;
        }

        public bool Authenticated { get; set; }
        public string Identity { get; set; }
    }
}
