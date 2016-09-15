using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;

namespace DocSupply.Controllers
{
    public class SignoutController : Controller
    {
        public IActionResult Index()
        {
            //to log out of auth, we need to call
            //http://test-docsupply-auth.nice.org.uk/auth/oa/logout
            //(we could remove this signin cookie just after log in, so we are never logged in to auth)

            //to log out of athens we nned to call
            //https://login.openathens.net/signout
            //can can do call the above calls in js, if requirement is to hard logout

            //lets just log out of doc supply for now, keep it simple, soft logout
            HttpContext.Authentication.SignOutAsync("ds");
            return Redirect("/Home");
        }

    }
}
