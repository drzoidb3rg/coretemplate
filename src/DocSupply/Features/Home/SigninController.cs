using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DocSupply.Framework;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Raven.Client;

namespace DocSupply.Controllers
{
    public class SigninController : Controller
    {
        private readonly IOptions<AppSettings> _optionsAccessor;

        public SigninController(IOptions<AppSettings> optionsAccessor)
        {
            _optionsAccessor = optionsAccessor;
        }

        public IActionResult Index()
        {
            var qs = Request.QueryString.ToString();

            var authUrl = _optionsAccessor.Value.AuthUrl;

            var thisHost = "http://" + Request.GetUri().Authority;

            qs = qs.ToLower().Replace("returnurl=", "returnurl=" + thisHost);

            var redirect = authUrl + qs;

            //http://test-docsupply-auth.nice.org.uk/protected.aspx?returnurl=http://localhost:56456/Signin/?ReturnUrl=%2Fconneg%2F1

            return Redirect(redirect);
        }

    }
}
