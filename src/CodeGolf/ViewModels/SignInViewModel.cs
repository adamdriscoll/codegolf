using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Authentication;

namespace CodeGolf.ViewModels
{
    public class SignInViewModel : AuthenticatedViewModel
    {
        public SignInViewModel(IEnumerable<AuthenticationDescription> descriptions) : base(false, string.Empty)
        {
            Descriptions = descriptions;
        }

        public IEnumerable<AuthenticationDescription> Descriptions { get; private set; }
    }
}
