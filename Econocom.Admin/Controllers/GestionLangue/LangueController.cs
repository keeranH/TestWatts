using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Econocom.Admin.Controllers.GestionLangue
{
    [Authorize]
    public class LangueController : Controller
    {
        //
        // GET: /Langue/

        public ActionResult Index()
        {
            return View();
        }

    }
}
