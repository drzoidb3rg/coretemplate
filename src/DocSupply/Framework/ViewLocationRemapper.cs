using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Razor;

namespace DocSupply.Framework
{
    public class ViewLocationRemapper : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations)
        {

            var temp = viewLocations.ToList();

            //need to understand this better and automate

            //if (context.ControllerName == "ConnegHandler")
            //{
            //    temp.Add("/Features/Conneg/Views/ConnegHypermedia.cshtml");
            //}

            //if (context.ControllerName == "DJListHandler")
            //{
            //    temp.Add("/Features/DJ/Views/DiscoveryJobListHypermedia.cshtml");
            //}

            //if (context.ControllerName == "DJHandler")
            //{
            //    temp.Add("/Features/DJ/Views/DiscoveryJobHypermedia.cshtml");
            //}

            return temp;

        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            // do nothing.. not entirely needed for this 
        }
    }

}
