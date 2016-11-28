using System;
using System.Threading.Tasks;
using CodeGolf.Models;
using CodeGolf.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class AuthorizedController : Controller
    {
        protected DocumentDbService DocumentDbService { get; }

        public AuthorizedController(DocumentDbService documentDbService)
        {
            DocumentDbService = documentDbService;
        }

        protected async Task<User> GetRequestUser()
        {
            if (!HttpContext.User.Identity.IsAuthenticated) return null;

            var user =
                await
                    DocumentDbService.Repository.Users.Get(HttpContext.User.Identity.Name,
                        HttpContext.User.Identity.AuthenticationType);

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Identity = HttpContext.User.Identity.Name,
                    Authentication = HttpContext.User.Identity.AuthenticationType
                };

                await DocumentDbService.Repository.Users.Create(user);
            }

            return user;
        }
    }
}
