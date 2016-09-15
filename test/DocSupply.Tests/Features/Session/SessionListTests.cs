using NUnit.Framework;
using System.Threading.Tasks;
using DocSupply.Models;
using System;
using System.Linq;
using DocSupply.Features.Session;
using DocSupply.Features.UserAccount;

namespace DocSupply.Tests
{
    [TestFixture]
    public class SessionLstTests
    {

        [TestCase]
        public void SignInProcess()
        {
            var key = "xxxxxxxx";

            var athensUser = new AthensUser { AthensPuid = "abc-123", FullName = "[full-name]" };

            var json = Encryption.Encrypt(athensUser.ToJson(), key);

            var dto = new AuthDTO {Data = json};

            using (var store = WebTest.GetDocumentStore())
            {
                //Session record created, token is returned
                TokenJson tokenJson;
                using (var _ = Tester.AsUnknown()
                       .WithEnvironmentVariable("AppSettings:AuthKey", key)
                       .WithStore(store)
                       .WithIndex(new UsersByAthensPuid())
                       .EnsureSuccessStatusCode()
                       .Post("/sessions", dto))
                {
                    tokenJson = _.Response.FromJson<TokenJson>();
                    tokenJson.token = Encryption.Decrypt(tokenJson.token, key);
                    
                    Assert.That(tokenJson.token.Length == 36);

                    var doc = _.GetFromStore<SessionRecord>("SessionRecord/" + tokenJson.token);
                    Assert.AreEqual("abc-123", doc.AthensUser.AthensPuid);
                };


                //get with token, redirect to home page, new user has been created
                using (var _ = Tester.AsUnknown()
                   .WithEnvironmentVariable("AppSettings:AuthKey", "XXXXXX")
                   .WithStore(store)
                   .WithIndex(new UsersByAthensPuid())
                   .EnsureSuccessStatusCode()
                   .Get("/session?t=" + tokenJson.token + "&returnurl=/home")
                   .IsFound())
                {
                    Assert.AreEqual(_.Response.Headers.Location.ToString(), "/home");
                    var doc = _.GetFromStore<User>("User/1");
                    Assert.AreEqual("abc-123", doc.AthensPuid);
                }


                //check the token can only be used once
                using (var _ = Tester.AsUnknown()
                    .WithEnvironmentVariable("AppSettings:AuthKey", "XXXXXX")
                    .WithStore(store)
                    .WithIndex(new UsersByAthensPuid())
                    .EnsureSuccessStatusCode()
                    .Get("/session?t=" + tokenJson.token + "&returnurl=/home")
                    .IsBadRequest()){}


                //check we cannot re-play a request
                using (var _ = Tester.AsUnknown()
                   .WithEnvironmentVariable("AppSettings:AuthKey", key)
                   .WithStore(store)
                   .WithIndex(new UsersByAthensPuid())
                   .EnsureSuccessStatusCode()
                   .Post("/sessions", dto)
                   .IsBadRequest()){ }
            }

        }

        [TestCase]
        public void StoreSessionIdsOnUserRecord()
        {
            var user = new User();

            for (int i = 0; i < 35; i++)
                user.AddSessionId(Guid.NewGuid().ToString());

            user.AddSessionId("xx");

            Assert.AreEqual(30, user.LastSessionIds.Count);

            Assert.AreEqual("xx", user.LastSessionIds.FirstOrDefault());
        }

        [TestCase]
        public void PostSessionsStaleRequest()
        {
            var key = "xxxxxxxx";

            var athensUser = new AthensUser { AthensPuid = "abc-123", FullName = "[full-name]", DateStamp = DateTime.UtcNow.AddMinutes(-130).ToString() };

            var json = Encryption.Encrypt(athensUser.ToJson(), key);

            var dto = new AuthDTO { Data = json };

            using (var _ = Tester.AsUnknown()
                   .WithEnvironmentVariable("AppSettings:AuthKey", key)
                   .Post("/sessions", dto)
                   .IsBadRequest()) { };
        }
    }
}

