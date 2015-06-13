using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Econocom.Admin.Controllers.GestionSouscription
{
    [Authorize]
    public class SouscriptionController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
