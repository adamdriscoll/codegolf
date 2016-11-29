using System.Security.Principal;

namespace CodeGolf.ViewModels
{
    public class DrivingRangeViewModel : AuthenticatedViewModel
    {
        public DrivingRangeViewModel(IIdentity identity) : base(identity)
        {
        }
    }
}
