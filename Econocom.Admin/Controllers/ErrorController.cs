using System;
using System.Web.Mvc;
using Econocom.Model;
using Infrastructure.DTO;


namespace Econocom.Web4.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(Exception error)
        {
            if(Request.IsAjaxRequest())
            {
                if (error is EconocomException)
                    return PartialView("Expectedp", new ErrorDisplay { Message = error.Message });
                return PartialView("Errorp", new ErrorDisplay { Message = error.Message });
            }

            if (error is EconocomException)
                return View("Expected", new ErrorDisplay { Message = error.Message });
            return View("Error", new ErrorDisplay{Message = error.Message});
        }

        public ActionResult HttpError404(Exception error)
        {
            return View();
        }

        public ActionResult HttpError505(Exception error)
        {
            return View();
        }
    }
}