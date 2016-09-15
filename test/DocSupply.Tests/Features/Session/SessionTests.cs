using NUnit.Framework;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;


namespace DocSupply.Tests
{
    [TestFixture]
    public class SessionTests
    {

        [TestCase]
        public void GetSessionWithNoToken()
        {
            using (var _ = Tester.AsUnknown()
                   .Get("/session")
                   .IsBadRequest()) { };
        }

    }
}
