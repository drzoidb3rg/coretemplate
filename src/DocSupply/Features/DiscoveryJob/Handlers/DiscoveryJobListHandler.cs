using System.Collections.Generic;
using System.Linq;
using DocSupply.Framework;
using DocSupply.RisParser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Raven.Client;

namespace DocSupply.Features.DiscoveryJob
{

    [Route("discoveryjoblist")]
    [Authorize(Policy = "SignedIn")]
    public class DiscoveryJobListHandler : Handler
    {
        IDocumentStore _store;
        private readonly IOptions<AppSettings> _optionsAccessor;

        public DiscoveryJobListHandler(IDocumentStore store, IOptions<AppSettings> optionsAccessor) 
        {
            _store = store;
            _optionsAccessor = optionsAccessor;
        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            //can DocSupplyPrinciple be injected ?
            var user = new DocSupplyPrinciple(HttpContext.User);

            var list = new List<DiscoveryJobRecord>();


            //this will only retrieve 128 records. place holder code for now
            using (var session = _store.OpenSession())
            {
                list = session.Query<DiscoveryJobRecord, DiscoveryJobRecordsByUser>()
                                 .Where(x => x.UserId == user.Id)
                                 .ToList();
            }

            var model = new DiscoveryJobListHypermedia(list) {Title = "Discovery job list"};
            model.AddPost();

            return Ok(model);
        }

        [HttpPost]
        public IActionResult Post(IFormCollection collection)
        {
            var user = new DocSupplyPrinciple(HttpContext.User);

            var firstFile = collection.Files.FirstOrDefault();

            var discoveryJobRecord = CreateDiscovery(firstFile);

            //need to handle this better
            if(discoveryJobRecord == null)
                return Redirect("/discoveryjoblist");

            discoveryJobRecord.UserId = user.Id;
            discoveryJobRecord.UserName = user.Name;

            _store.Store(discoveryJobRecord);

            return Redirect(string.Format(@"discoveryjob\{0}", discoveryJobRecord.GetIntegerId()));
        }



        private DiscoveryJobRecord CreateDiscovery(IFormFile file)
        {
            if (file.Length > 0)
            {
                var risFile = RisFileParser.ParseRisFile(file);
                return risFile.Valid ? new DiscoveryJobRecord(file.FileName, risFile) : null;
            }
            return null;
        }

    }
}
