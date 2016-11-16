using CodeGolf.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace CodeGolf.ViewModels
{
    public class UserViewModel
    {
        public UserViewModel(User user, string currentUser, IUrlHelper uriHelper)
        {
            Name = user.Identity;
            AuthType = user.Authentication;
            IsCurrentUser = currentUser == Name;

            var ac = new UrlActionContext
            {
                Action = "Get",
                Controller = "Profile",
                Values = new {id = user.Id}
            };

            ProfileUrl = uriHelper.Action(ac);
        }

        public bool IsCurrentUser { get; set; }

        public string Name { get; set; }

        public string AuthType { get; set; }

        public string ProfileUrl { get; set; }
    }
}
