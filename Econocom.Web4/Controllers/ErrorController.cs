using System;
using System.Web.Mvc;
using Econocom.Model;
using Infrastructure.DTO;


namespace Econocom.Web4.Controllers
{
    public class ErrorController : BaseController
    {
        public ActionResult Index(Exception error)
        {            
            return View("Error", new ErrorDisplay{Message = error.Message});
        }

        public ActionResult HttpError403(Exception error)
        {
            return View("HttpError403", new ErrorDisplay { Message = error.Message });

        }
        public ActionResult HttpError404(Exception error)
        {
            return View("HttpError404", new ErrorDisplay { Message = error.Message });
        }

        public ActionResult HttpError505(Exception error)
        {
            return View("HttpError505", new ErrorDisplay { Message = error.Message });
        }
    }
}