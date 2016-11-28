using System;
using System.Collections.Generic;
using System.Linq;
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

            var user = DocumentDbService.Client.CreateDocumentQuery<User>(DocumentDbService.DatabaseUri).Where(m => m.Identity == this.HttpContext.User.Identity.Name && m.Authentication == this.HttpContext.User.Identity.AuthenticationType).ToList().FirstOrDefault();
            if (user == null)
            {
                user = new User
                {
                    Identity = this.HttpContext.User.Identity.Name,
                    Authentication = this.HttpContext.User.Identity.AuthenticationType
                };

                await DocumentDbService.CreateDocument(user);
            }

            return user;
        }
    }
}
