using System.Security.Principal;

namespace CodeGolf.ViewModels
{
    public class AuthenticatedViewModel
    {
        public AuthenticatedViewModel(IIdentity identity) : this(identity.IsAuthenticated, identity.Name)
        {
            
        }

        public AuthenticatedViewModel(bool authenticated, string identity = null)
        {
            Authenticated = authenticated;
            Identity = identity;
        }

        public bool Authenticated { get; set; }
        public string Identity { get; set; }
    }
}
