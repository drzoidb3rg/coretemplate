using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocSupply.Models;

namespace DocSupply.Features.UserAccount
{
    public class User : RavenRecord
    {
        
        public string AthensOrgName { get; set; }
        public string AthensPuid { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }

        public List<string> LastSessionIds { get; set; }

        public DateTime LastSignIn { get; set; }

        public User()
        {
            LastSessionIds = new List<string>();
        }

        public User(AthensUser athensUser) : this()
        {
            AthensOrgName = athensUser.AthensOrgName;
            AthensPuid = athensUser.AthensPuid;
            Email = athensUser.Email;
            FullName = athensUser.FullName;
        }

        public void UpdateFromAthens(AthensUser athensUser)
        {
            if(athensUser.Email != Email)
                Email = athensUser.Email;

            if (athensUser.FullName != FullName)
                FullName = athensUser.FullName;
        }

        public void AddSessionId(string sessionId)
        {
            LastSessionIds.Insert(0, sessionId);

            if (LastSessionIds.Count > 30)
                LastSessionIds.RemoveRange(30, LastSessionIds.Count - 30);
        }

        public bool ContainsSession(string sessionId)
        {
            return LastSessionIds.Contains(sessionId);
        }
    }
}
