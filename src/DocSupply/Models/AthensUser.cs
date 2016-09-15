using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocSupply.Models
{
    [Serializable]
    public class AthensUser
    {
        public string Address { get; set; }
        public string AdminEmail { get; set; }
        public string AthensAccountExpiry { get; set; }
        public string AthensAuthenticated { get; set; }
        public string AthensAuthorised { get; set; }
        public string AthensGroup { get; set; }
        public string AthensId { get; set; }
        public string AthensLToken { get; set; }
        public string AthensOrgId { get; set; }
        public string AthensOrgName { get; set; }
        public string AthensPuid { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string NlhUserId { get; set; }
        public string Phone { get; set; }
        public string Id { get; set; }
        public string DateStamp { get; set; }
        public List<NlhUserRole> UserRoles { get; set; }


        public AthensUser()
        {
            Id = Guid.NewGuid().ToString();
            DateStamp = DateTime.UtcNow.ToString();
        }
    }

    [Serializable]
    public enum NlhUserRole
    {
        /// <summary>
        /// Can administer the system
        /// </summary>
        Administrator = 1,

        /// <summary>
        /// Can view log files
        /// </summary>
        LogViewer = 2
    }
}
