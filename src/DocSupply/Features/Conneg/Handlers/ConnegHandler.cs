using DocSupply.Features.Conneg.Models;
using DocSupply.Framework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocSupply.Features.Conneg
{

    [Route("conneg/{id}")]
    [Authorize(Policy = "SignedIn")]
    public class ConnegHandler : Handler
    {

        private Conneg Retrieve(int id)
        {
            var model = new Conneg
            {
                Title = "some title",
                Id = "/conneg/" + id
            };

            return model;
        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            //this typically comes from database
            var model = Retrieve(id);

            var media = new ConnegHypermedia(model);
            media.AddPut();

            return Ok(media);
        }

        [HttpPut]
        public IActionResult Put(int id, [FromForm] Conneg data)
        {
            //this typically comes from database
            var model = Retrieve(id);

            //update 
            model.Title = data.Title;

            var media = new ConnegHypermedia(model);
            media.AddPut();

            return Ok(media);
        }

        [HttpPost]
        public IActionResult Post(int id, [FromForm] Conneg data)
        {
            //this typically comes from database
            var model = Retrieve(id);

            //update 
            model.Title = data.Title;

            var media = new ConnegHypermedia(model);
            media.AddPut();

            return Ok(media);
        }
    }
}
