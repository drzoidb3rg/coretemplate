using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocSupply.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Raven.Client;

namespace DocSupply.Features.DiscoveryJob
{

    [Route("discoveryjob/{id}")]
    [Authorize(Policy = "SignedIn")]
    public class DiscoveryJobHandler : Handler
    { 
        IDocumentStore _store;
        private readonly IOptions<AppSettings> _optionsAccessor;

        public DiscoveryJobHandler(IDocumentStore store, IOptions<AppSettings> optionsAccessor)
        {
            _store = store;
            _optionsAccessor = optionsAccessor;
        }


        [HttpGet]
        public IActionResult Get(int id)
        {
            //this typically comes from database
            var job =_store.Load<DiscoveryJobRecord>("DiscoveryJobRecord/" + id);

            var media = new DiscoveryJobHypermedia(job);
           
            return Ok(media);
        }

    }
}
