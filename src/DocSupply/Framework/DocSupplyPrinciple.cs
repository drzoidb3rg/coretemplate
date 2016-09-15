using System.Linq;
using System.Security.Claims;

namespace DocSupply.Framework
{
    public class DocSupplyPrinciple
    {
        public string Id { get; set; }
        public string Name { get; set; }


        public DocSupplyPrinciple()
        {
            Id = "-1";
            Name = "not logged in";
        }

        public DocSupplyPrinciple(ClaimsPrincipal claimsPrincipal) : this()
        {
            if(claimsPrincipal?.Claims.FirstOrDefault(x => x.Type == "Id") == null)
                return;

            Id = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;
            Name = claimsPrincipal.Identity.Name;
        }
    }
}