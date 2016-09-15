using System;
using System.Security.Claims;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace DocSupply.Tests
{
    public static class Tester
    {
        public static WebTest AsUnknown()
        {
            return new WebTest();
        }

        public static WebTest AsUser(string userName, string userId)
        {
            return new WebTest()
                .WithRequestHeader("Test-UserName", userName)
                .WithRequestHeader("Test-Id", userId);
        }

        public static WebTest AsUserOne()
        {
            return AsUser("user one", "1");
        }

        public static WebTest AsUserTwo()
        {
            return AsUser("user two", "2");
        }
    }
}