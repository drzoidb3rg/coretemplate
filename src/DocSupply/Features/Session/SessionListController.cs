using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocSupply.Features.UserAccount;
using DocSupply.Framework;
using Microsoft.AspNetCore.Mvc;
using DocSupply.Models;
using Microsoft.Extensions.Options;
using NuGet.Protocol.Core.v3;
using Raven.Client;

namespace DocSupply.Features.Session
{
    [Serializable]
    public class AuthDTO
    {
        public string Data { get; set; }
    }

    [Route("sessions")]
    public class SessionListController : Controller
    {
        IDocumentStore _store;
        private readonly IOptions<AppSettings> _optionsAccessor;

        public SessionListController (IDocumentStore store, IOptions<AppSettings> optionsAccessor)
        {
            _store = store;
            _optionsAccessor = optionsAccessor;
        }

        [HttpPost]
        public IActionResult Index([FromBody] AuthDTO dto)
        {

            var key = _optionsAccessor.Value.AuthKey;

            var json = Encryption.Decrypt(dto.Data, key);

            var athensUser = json.FromJson<AthensUser>();

            //basic checks to exclude obviously  stale request
            if (athensUser.DateStamp.EmptyString())
            {
                //return Json("empty date stamp");
                return BadRequest();
            }

            var totalMinutes = (DateTime.UtcNow - DateTime.Parse(athensUser.DateStamp)).TotalMinutes;

            if (!(totalMinutes < 120 && totalMinutes > - 120))
            {
                //return Json("totalminutes:" + totalMinutes +  ";thistime:" + DateTime.UtcNow + ";authtime:" + athensUser.DateStamp);
                return BadRequest();
            }

            var token = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");
           
            var sessionRecord = new SessionRecord { AthensUser = athensUser, Id = "SessionRecord/" + token };

            using (var session = _store.OpenSession())
            {
                var user = session.Query<User, UsersByAthensPuid>().FirstOrDefault(x => x.AthensPuid == sessionRecord.AthensUser.AthensPuid);

                if (user != null)
                {
                    if(user.ContainsSession(athensUser.Id))
                        return BadRequest();
                }

                session.Store(sessionRecord);
                session.SaveChanges();
            }

            var tokenJson = new TokenJson { token = Encryption.Encrypt(token,key) };

            return Json(tokenJson);
        }


    }
}
