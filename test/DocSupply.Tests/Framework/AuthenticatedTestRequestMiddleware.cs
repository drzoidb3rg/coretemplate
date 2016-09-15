using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DocSupply.Tests.Framework
{
    public class AuthenticatedTestRequestMiddleware
    {
        public const string TestingCookieAuthentication = "TestCookieAuthentication";
        public const string TestingHeader = "X-Integration-Testing";

        private readonly RequestDelegate _next;

        public AuthenticatedTestRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.Keys.Contains("Test-UserName"))
            {
                var userName = context.Request.Headers["Test-UserName"];
                var id = context.Request.Headers["Test-Id"];

                var claims = new[] {new Claim(ClaimTypes.Name, userName), new Claim("Id", id)};

                var identity = new ClaimsIdentity(claims, "ds");

                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                context.User = claimsPrincipal;
            }

            await _next(context);
        }
    }
}
