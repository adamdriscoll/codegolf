using CodeGolf.Models;

namespace CodeGolf.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel(Sql.Models.User user, string currentUser)
        {
            Name = user.Identity;
            AuthType = user.Authentication;
            IsCurrentUser = currentUser == Name;
            ProfileUrl = $"/profile/{user.Identity}";
        }

        public bool IsCurrentUser { get; set; }

        public string Name { get; set; }

        public string AuthType { get; set; }

        public string ProfileUrl { get; set; }
    }
}
