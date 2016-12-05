using System;
using System.Threading.Tasks;
using CodeGolf.Sql.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class AuthorizedController : Controller
    {
        protected IRepository Repository { get; }

        public AuthorizedController(IRepository repository)
        {
            Repository = repository;
        }

        protected async Task<Sql.Models.User> GetRequestUser()
        {
            if (!HttpContext.User.Identity.IsAuthenticated) return null;

            var user =
                await
                    Repository.Users.Get(HttpContext.User.Identity.Name,
                        HttpContext.User.Identity.AuthenticationType);

            if (user == null)
            {
                user = new Sql.Models.User
                {
                    Identity = HttpContext.User.Identity.Name,
                    Authentication = HttpContext.User.Identity.AuthenticationType,
                    DateAdded = DateTime.UtcNow
                };

                await Repository.Users.Create(user);
            }

            return user;
        }
    }
}
