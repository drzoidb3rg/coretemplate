using System;
using System.Security.Claims;
using DocSupply.Features.UserAccount;
using DocSupply.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Raven.Client;

namespace DocSupply.Controllers
{
    public class SignindevController : Controller
    {

        IDocumentStore _store;
        private readonly IOptions<AppSettings> _optionsAccessor;

        public SignindevController(IDocumentStore store, IOptions<AppSettings> optionsAccessor)
        {
            _store = store;
            _optionsAccessor = optionsAccessor;
        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }



        public IActionResult UserOne()
        {
            var user = _store.Load<User>("User/1");

            if (user == null)
            {
                user = new User
                {
                    AthensOrgName = "IM&T",
                    AthensPuid = "01141204:01243aa",
                    Email = "user.one@docsupply.nice.org.uk",
                    FullName = "User One",
                    Id = "user/1"
                };

                _store.Store(user);
            }

            DoCookie(user);

            return Redirect("/Home");
        }

        public IActionResult UserTwo()
        {
            var user = _store.Load<User>("User/2");

            if (user == null)
            {
                user = new User
                {
                    AthensOrgName = "IM&T",
                    AthensPuid = "01141204:01243aa",
                    Email = "user.two@docsupply.nice.org.uk",
                    FullName = "User Two",
                    Id = "user/2"
                };

                _store.Store(user);
            }

            DoCookie(user);

            return Redirect("/Home");
        }


        private void DoCookie(User user)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, user.FullName), new Claim("Id", user.GetIntegerId()) };

            var identity = new ClaimsIdentity(claims, "ds");

            var cookie = HttpContext.Authentication.SignInAsync("ds", new ClaimsPrincipal(identity));
        }
    }
}
