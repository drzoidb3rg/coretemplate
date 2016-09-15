using Microsoft.AspNetCore.Mvc;

namespace DocSupply.Framework
{
    public class Handler : Controller
    {
        public IActionResult Ok(object model)
        {
            var accept = Request.Headers["accept"];

            if (accept == "application/json")
                return base.Ok(model);

            var viewName = model.GetType().Name;

            return View(viewName, model);
        } 

    }
}