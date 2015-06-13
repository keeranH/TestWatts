using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Econocom.Admin.Controllers.GestionIndicateur
{
    [Authorize]
    public class IndicateurController : Controller
    {
        //
        // GET: /Indicateur/

        public ActionResult Index()
        {
            return View();
        }

    }
}
