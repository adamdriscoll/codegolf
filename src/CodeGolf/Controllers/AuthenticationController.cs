using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeGolf.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CodeGolf.Controllers
{
    public class AuthenticationController : Controller
    {
        [HttpGet("~/signin")]
        public IActionResult SignIn() => View("SignIn", new SignInViewModel(HttpContext.GetExternalProviders()));

        [HttpPost("~/signin")]
        public IActionResult SignIn([FromForm] string provider)
        {
            // Note: the "provider" parameter corresponds to the external
            // authentication provider choosen by the user agent.
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest();
            }

            if (!HttpContext.IsProviderSupported(provider))
            {
                return BadRequest();
            }

            // Instruct the middleware corresponding to the requested external identity
            // provider to redirect the user agent to its own authorization endpoint.
            // Note: the authenticationScheme parameter must match the value configured in Startup.cs
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, provider);
        }

        [HttpGet("~/signout"), HttpPost("~/signout")]
        public IActionResult SignOut()
        {
            // Instruct the cookies middleware to delete the local cookie created
            // when the user agent is redirected from the external identity provider
            // after a successful authentication flow (e.g Google or Facebook).
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }

    public static class HttpContextExtensions
    {
        public static IEnumerable<AuthenticationDescription> GetExternalProviders(this HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return from description in context.Authentication.GetAuthenticationSchemes()
                   where !string.IsNullOrWhiteSpace(description.DisplayName)
                   select description;
        }

        public static bool IsProviderSupported(this HttpContext context, string provider)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (from description in context.GetExternalProviders()
                    where string.Equals(description.AuthenticationScheme, provider, StringComparison.OrdinalIgnoreCase)
                    select description).Any();
        }
    }
}
