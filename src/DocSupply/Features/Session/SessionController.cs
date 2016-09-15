
using Microsoft.AspNetCore.Mvc;
using Raven.Client;
using DocSupply.Features.UserAccount;
using System.Linq;
using System;
using System.Security.Claims;

namespace DocSupply.Features.Session
{

    [Route("session")]
    public class SessionController : Controller
    {
        IDocumentStore _store;


        public SessionController(IDocumentStore store)
        {
            _store = store;
        }

        [HttpGet]
        public IActionResult Get(string t, string returnUrl)
        {
            if (string.IsNullOrEmpty(t))
                return BadRequest();

            if (returnUrl == null)
                returnUrl = "";

            User user;

            using (var session = _store.OpenSession())
            {
                var sessionRecord = session.Load<SessionRecord>("SessionRecord/" + t);

                if (sessionRecord == null)
                    return BadRequest();

                user = session.Query<User, UsersByAthensPuid>().FirstOrDefault(x => x.AthensPuid == sessionRecord.AthensUser.AthensPuid);

                if (user == null)
                    user = new User(sessionRecord.AthensUser);
                else
                    user.UpdateFromAthens(sessionRecord.AthensUser);

                if (user.ContainsSession(sessionRecord.AthensUser.Id))
                    return BadRequest();

                user.AddSessionId(sessionRecord.AthensUser.Id);

                user.LastSignIn = DateTime.UtcNow;

                session.Store(user);

                session.Delete(sessionRecord);

                session.SaveChanges();
            }

            var claims = new[] { new Claim(ClaimTypes.Name, user.FullName), new Claim("Id", user.Id) };

            var identity = new ClaimsIdentity(claims, "ds");

            var cookie = HttpContext.Authentication.SignInAsync("ds", new ClaimsPrincipal(identity));

            //ensure return url is relative
            returnUrl = returnUrl.Replace("http://" + Request.Host, "");

            return LocalRedirect(returnUrl);
        }  

    }
}
