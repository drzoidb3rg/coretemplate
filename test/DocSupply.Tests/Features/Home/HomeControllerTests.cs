using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using Raven.Database;

using NUnit.Framework;

namespace DocSupply.Tests.Features.Home
{
    [TestFixture]
    public class HomeControllerTests 
    {


        [TestCase]
        public void TestOne()
        {
            //var x = new WebTest().GetServer2();



            var user = new DocSupply.Models.AthensUser { Id = "99" };

            Assert.IsTrue(user.Id == "99");
        }

    }
}
